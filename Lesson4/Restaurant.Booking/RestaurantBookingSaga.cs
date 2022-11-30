using System;
using Automatonymous;
using MassTransit;
using Restaurant.Booking.Consumers;
using Restaurant.Messages;

namespace Restaurant.Booking
{
    public sealed class RestaurantBookingSaga : MassTransitStateMachine<RestaurantBooking>
    {
        public RestaurantBookingSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => BookingRequested, x => x.CorrelateById(context => context.Message.OrderId).SelectId(context => context.Message.OrderId));
            
            Event(() => TableBooked, x => x.CorrelateById(context => context.Message.OrderId));

            Event(() => KitchenReady,  x => x.CorrelateById(context => context.Message.OrderId));

            Event(() => BookingCancellation, x => x.CorrelateById(context => context.Message.OrderId));

            Event(() => GuestArrived, x => x.CorrelateById(context => context.Message.OrderId));

            CompositeEvent(() => BookingApproved, 
                x => x.ReadyEventStatus, KitchenReady, TableBooked);

            Event(() => BookingRequestFault,
                x => x.CorrelateById(m => m.Message.Message.OrderId));

            Schedule(() => BookingExpired,
                x => x.ExpirationId, x =>
            {
                x.Delay = TimeSpan.FromSeconds(5);
                x.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });

            Schedule(() => GuestWaitingExpired,
                x => x.ExpirationId, x =>
                {
                    x.Delay = TimeSpan.FromSeconds(5);
                    x.Received = e => e.CorrelateById(context => context.Message.OrderId);
                });

            Initially(
                When(BookingRequested)
                    .Then(context =>
                    {
                        context.Instance.CorrelationId = context.Data.OrderId;
                        context.Instance.OrderId = context.Data.OrderId;
                        context.Instance.ClientId = context.Data.ClientId;
                        Console.WriteLine("Saga: " + context.Data.CreationDate);
                    })
                    .Schedule(BookingExpired, 
                        context => new BookingExpire (context.Instance),
                        context => TimeSpan.FromSeconds(1))
                    .TransitionTo(AwaitingBookingApproved)
            );

            During(AwaitingBookingApproved,
                When(BookingApproved)
                    .Unschedule(BookingExpired)
                    .Publish(context =>
                        (INotify) new Notify(context.Instance.OrderId,
                            context.Instance.ClientId,
                            $"Стол успешно забронирован"))
                    .TransitionTo(AwaitingGuestArrival),

                When(BookingRequestFault)
                    .Then(context => Console.WriteLine($"Ошибочка вышла!"))
                    .Publish(context => (INotify) new Notify(context.Instance.OrderId,
                        context.Instance.ClientId,
                        $"Приносим извинения, стол забронировать не получилось."))
                    .Publish(context => (IBookingCancellation) 
                        new BookingCancellation(context.Data.Message.OrderId))
                    .Finalize(),
                
                When(BookingExpired.Received)
                    .Then(context => Console.WriteLine($"Отмена заказа {context.Instance.OrderId}"))
                    .Publish(context => (IBookingCancellation)
                        new BookingCancellation(context.Instance.OrderId))
                    .Finalize()
            );

            During(AwaitingGuestArrival,
                When(GuestArrived)
                    .Unschedule(GuestWaitingExpired)
                    .Publish(context =>
                        (INotify)new Notify(context.Instance.OrderId,
                            context.Instance.ClientId,
                            $"Стол занят - гость прибыл"))
                    .Finalize(),

                When(BookingCancellationRequest)
                    .Unschedule(GuestWaitingExpired)
                    .Publish(context => (INotify)new Notify(context.Instance.OrderId,
                        context.Instance.ClientId,
                        $"Клиент отменил заказ."))
                    .Publish(context => (IBookingCancellation)
                        new BookingCancellation(context.Data.OrderId))
                    .Finalize(),

                When(GuestWaitingExpired.Received)
                    .Then(context => Console.WriteLine($"Гость вовремя не прибыл {context.Instance.OrderId}"))
                    .Publish(context => (IBookingCancellation)
                        new BookingCancellation(context.Instance.OrderId))
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
        public State AwaitingBookingApproved { get; private set; }
        public State AwaitingGuestArrival { get; private set; }

        public Event<IBookingRequest> BookingRequested { get; private set; }
        public Event<ITableBooked> TableBooked { get; private set; }
        public Event<IKitchenReady> KitchenReady { get; private set; }
        public Event<IBookingCancellation> BookingCancellation { get; private set; }
        public Event<IBookingCancellationRequest> BookingCancellationRequest { get; private set; }
        public Event<IGuestArrived> GuestArrived { get; private set; }

        public Event<Fault<IBookingRequest>> BookingRequestFault { get; private set; }

        public Schedule<RestaurantBooking, IBookingExpire> BookingExpired { get; private set; }
        public Schedule<RestaurantBooking, IGuestWaitingExpire> GuestWaitingExpired { get; private set; }

        public Event BookingApproved { get; private set;  }
    }
}
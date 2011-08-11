﻿using Core.TestHelper.Extensions;
using EmailMaker.Domain.Events.Emails;
using EmailMaker.Messages;
using NServiceBus;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace EmailMaker.Domain.EventHandlers.Tests
{
    [TestFixture]
    public class when_handling_enqueue_email_to_be_sent_event
    {
        private IBus _bus;
        private int _emailId = 23;

        [SetUp]
        public void Context()
        {
            _bus = MockRepository.GenerateMock<IBus>();

            var evnt = new EmailEnqueuedToBeSentEvent{ EmailId = _emailId };
            var handler = new EmailEnqueuedToBeSentEventHandler(_bus);
            handler.Handle(evnt);
        }

        [Test]
        public void message_was_sent()
        {
            var message = _bus.MessageShouldHaveBeenSent<EmailEnqueuedToBeSentEventMessage>();
            message.EmailId.ShouldBe(_emailId);
        }
    }

}

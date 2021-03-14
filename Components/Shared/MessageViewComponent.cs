using System;
using leave_management.Extensions;
using leave_management.Models.Enums;
using leave_management.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace leave_management.Components.Shared
{
    public class MessageViewComponent : ViewComponent
    {
        public MessageViewComponent() {}

        public IViewComponentResult Invoke(
            string messageTitle = null,
            string messageContent = null, 
            MessageType messageType = MessageType.warning, 
            string messageDivId = "message-div")
        {
            var standardMessageVM = new StandardMessageVM(messageTitle, messageContent, messageType, messageDivId);
            var viewName = MvcHelper.NameOfViewComponent<MessageViewComponent>();

            return View($@"~/Views/Shared/Components/{viewName}.cshtml", standardMessageVM);
        }
    }
}
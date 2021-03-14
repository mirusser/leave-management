using leave_management.Models.Enums;

namespace leave_management.Models.ViewModels
{
    public class StandardMessageVM
    {
        public string MessageTitle { get; set; }
        public string MessageContent { get; set; }
        public MessageType MessageType { get; set; }
        public string MessageDivId { get; set; }

        public StandardMessageVM(
            string messageTitle = null,
            string messageContent = null, 
            MessageType messageType = MessageType.warning, 
            string messageDivId = "message-div")
        {
            MessageTitle = messageTitle;
            MessageContent = messageContent;
            MessageType = messageType;
            MessageDivId = !string.IsNullOrEmpty(messageDivId) ? messageDivId : "message-div";
        }
    }
}
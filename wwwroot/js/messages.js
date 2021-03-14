
function standardMessage(messageTitle, messageContent, messageType, messageDivId) {
    var innerHtml = '';

    if (messageTitle) {
        innerHtml = '<div class="alert alert-' + messageType + ' alert-dismissible fade show" role="alert">' +
            '<h4 class="alert-heading">' +
            messageTitle +
            '</h4>' +
            '<p>' +
            messageContent +
            '</p>' +
            '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '</div>';
    }
    else {

        innerHtml = '<div class="alert alert-' + messageType + ' alert-dismissible fade show" role="alert">' +
            '<p>' +
            messageContent +
            '</p>' +
            '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '</div>';
    }

    $("#" + messageDivId).html(innerHtml);
}

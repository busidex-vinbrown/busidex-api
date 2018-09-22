
$(document).ready(function() {    
    
    $("#sendTellAFriend").click(function () {
        var email = $("#inviteEmail").val();
        var name = $("#inviteName").val();
        if (email.length > 0 && name.length > 0) {
            $.ajax({
                url: '/account/SendInviteEmail',
                data: { name: name, email: email },
                complete: function() {
                    $("#inviteEmail").val('');
                    $("#inviteName").val('');
                }
            });
        }
    });

    $("#submitSharedCards").click(function() {
        $("#AddSharedCardsToMyBusidexForm").submit();
    });
    $("#flyout").flyout({ "edge": "-2", "top": "70", "handle": "flyoutHandle" });
});

function disableSave() {
    $("#submitLink").attr("disabled", "disabled");
}

function enableSave() {
    $("#submitLink").removeAttr("disabled");
}
$.fn.flyout = function (options) {
    
    var self = this;
    var edge = options.edge || 30;
    var top = options.top || 20;
    
    self.handle = $("#" + options.handle);
    
    self.css({
        top: top + "px",
        left: -1 * (self.width() - edge + 20)
    });
    self.handle.toggle(function () {
        $(self).animate({ left: "-15px" });
    }, function () {
        $(self).animate({ left: -1 * (self.width() - edge + 20) });
    });
}
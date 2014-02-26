$(function() {
    $("a.ajax-post").click(function () {
        var self = $(this);
        $.post(self.attr("href"), null, function (data) {
            self.parent().append(data);
            self.remove();
        });
        return false;
    });
});
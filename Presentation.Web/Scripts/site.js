﻿$(function() {
    $("a.ajax-post").click(function () {
        var btn = $(this);
        btn.button('loading');
        var parent = btn.parent();
        $.post(btn.attr("href"), null, function (data) {
            btn.remove();
            parent.append(data);
        },"text");
        
        return false;
    });
});
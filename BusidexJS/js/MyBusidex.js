
var BUSIDEX = (function() {
    
    var defaultHHeight = '150px';
    var defaultVHeight = '235px';
    var vertical = $(this).find('img').hasClass('v_preview');
    var sharing = false;
    
    var saveSharedCards = function() {

        var cardIds = new Array();
        var email = $("#shareWithEmail").val();
        
        $("input.sharedCard").each(function () {
            if ($(this).is(":checked")) {
                cardIds.push($(this).attr("cardId"));
            }
        });
        $.ajax({
            url: 'SendSharedCards',
            data: { email: email, cards: cardIds.join(',') },
            complete: function() {
                $("#shareWithEmail").val('');
            }
        });
    };

    var quickFind = function(t) {
        $(".cardContainer").each(function() {
            var content = $(this).html();

            if (content.toLowerCase().indexOf(t.toLowerCase()) >= 0) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    };

    var find = function(arr, item) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i] === item) {
                return true;
            }
        }
        return false;
    };

    var cover = function () {
        
        var el = $("div.detailRow.selected").eq(0);

        $(el).find("div.span3").removeClass("span3").addClass("span5");
        $(el).find("div.span9").removeClass("span9").addClass("span7");
        
        $(el).find("div.cardActions").show();
        $(el).attr('old-height', $(el).height());
        $("div.detailRow.selected").removeClass("selected").css({ 'height': vertical ? defaultVHeight : defaultHHeight });
        $("div.detailRow").css({ opacity: .25 });
        $('html, body').animate({
            scrollTop: $(el).offset().top - 150
        }, 1000);

        $(el).animate({ height: '400px', opacity: 1.0 }, 500).addClass('swell').addClass("selected");

        $(el).find("div.relatedCards").show();
    };

    var removeCover = function () {

        var el = $("div.detailRow.selected").eq(0);

        if (el.length == 0) return;
        
        $(el).find("div.relatedCards").hide();
        $(el).find("div.span5").removeClass("span5").addClass("span3");
        $(el).find("div.span7").removeClass("span7").addClass("span9");

        $(el).animate({ 'height': vertical ? defaultVHeight : defaultHHeight }, 500, function() {
            $(this).css('height', 'auto');
        }).removeClass("selected").removeClass('well').removeClass('selected');

        $("div.detailRow").css({ opacity: 1.0 });
        $("div.cardActions").hide();
    };
    
    return {
        init: function() {

            /*SHARING*/
            $(document).on("click", "a.share", function () {
                $("a.share").hide();
                $("a.cancel, a.select, div.selectShare").show();
                removeCover();
                sharing = true;
            });
            $(document).on("click", "a.cancel, #sendShared", function () {
                removeCover();
                $("a.share").show();
                $("a.cancel, a.select, div.selectShare").hide();
                sharing = false;
            });
            
            $(document).on("click", "a.cancel", function () {
                removeCover();
                $("div.selectShare input").attr("checked", false);
            });
            $(document).on("click", "#sendShared", function () {
                saveSharedCards();
                $("div.selectShare input").attr("checked", false);
                removeCover();
            });
            /*END SHARING*/

            $(document).on("click", "i.unrelated", function () {

                var userId = $("#ownerId").val();
                var cardId = $(this).attr("cardId");
                var el = $(this);
                $.ajax({
                    url: 'RelateCards',
                    data: { ownerId: userId, relatedCardId: cardId },
                    success: function() {
                        $(el).removeClass('unrelated').addClass('related').attr("title", "This card is related to yours");
                    }
                });
                return false;
            });
            
            $(document).on("click","i.related", function () {

                var userId = $("#ownerId").val();
                var cardId = $(this).attr("cardId");
                var el = $(this);
                $.ajax({
                    url: 'UnRelateCards',
                    data: { ownerId: userId, relatedCardId: cardId },
                    success: function () {
                        $(el).removeClass('related').addClass('unrelated').attr("title", "This card is not related to yours");
                    }
                });
                return false;
            });
            
            $("input.SelCat").each(function() {
                $(this).attr("checked", false);
            });

            //$(document).on("click", "textarea.Notes", function () {
            //    return false;
            //});
           
            //$(document).on("change", "textarea.Notes", function () {

            //    var data = { id: $(this).attr("ucId"), notes: escape($(this).val()) };

            //    $.ajax({
            //        url: '../Busidex/SaveCardNotes',
            //        data: data,
            //        cache: false,
            //        type: 'POST'
            //    });

            //});

            //$.fx.speeds._default = 900;
            //$("#detailPopup").dialog({
            //    autoOpen: false,
            //    height: 300,
            //    modal: true,
            //    resizable: false,
            //    show: 'blind',
            //    hide: 'blind',
            //    position: ['center', 140],
            //    width: 640
            //});

            $(document).on("click", 'a.removeCard', function() {
                if (confirm("Are you sure you want to remove this card from your Busidex collection (with all its notes)?")) {
                    var id = $(this).attr("cardId");
                    var item = $(this).parent().parent().parent();

                    $.ajax({
                        url: '../Card/DeleteUserCard/' + id,
                        success: function () {
                            removeCover();
                            $(item).hide('normal').remove();
                        } 
                    });
                }
                return false;
            });

            $(document).on("click", "#closePopup", function () {
                $("#cardPopup").slideUp('normal', function() {
                    $(this).find("img").each(function() {
                        $(this).remove();
                    });
                    $("#cover").hide();
                });
            });

            $(document).on("keyup", "input#quickfind", function() {
                var v = $(this).val();
                if (v.trim().length == 0) $(".cardContainer").show();
                else quickFind(v);
            });

            $(document).on("change", "#filter", function () {
                if ($(this).val() == "tags") {
                    $("input#filterVal").hide();
                    $("input#filterValTag").show();
                } else {
                    $("input#filterVal").show();
                    $("input#filterValTag").hide();
                }
            });

            function doFilter(val) {
                removeCover();
              if (val.length == 0) {
                    $("div.detailRow").show();
                } else {

                    var filter = $("#filter").val().trim();

                    $("div.detailRow").hide();
                    
                    var selector = "div.detailRow[" + filter + "*='" + val + "']";
                    $(selector).show();
                }  
            }
            
            $(document).on("keyup", "input#filterVal,input#filterValTag", function () {
                var val = $(this).val().toLowerCase().trim();
                doFilter(val);
            });

            $(document).on("click", "div.detailRow", function () {
                
               // window.location.hash = $(this).attr("cardId");
                if (!sharing) {
                    if (!$(this).hasClass('selected')) {
                        $(this).addClass('selected');
                        cover();
                    } 
                }
            });

            $(document).on("click", "a.removeCover", function () {
                removeCover();
                return false;
            });
        }
    };
})();

$(document).ready(function () {
    BUSIDEX.init();
    
    //if (window.location.hash) {
    //    var h = window.location.hash.replace('#', '');
    //    var t = $("a[idx=" + h + "]").offset().top - 300;
    //    $('html, body').animate({
    //        scrollTop: t
    //    }, 200);
    //}
});
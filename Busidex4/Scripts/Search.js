$(document).ready(function () {

    $("#help").click(function() {

        $.ajax({
            url: "/home/ShowMeHow/",
            data: { helpFile: 'FindACard.mp4' },
            success: function(help) {
                $("#helpContent").html(help);
            }
        });
    });
    $('.close').click(function() {
        $("#helpContent").html('');
        jwplayer().stop();
    });
    
    $("#Search a").css('color', '#fff');
    
    $(".section.content-wrapper").css("margin-left", "270px");
    
    $( "#search" ).focus();
    $( "#header" ).width( $( window ).width() );

    $( "#doSearch" ).click( function ()
    {
        return $( "#search" ).val().length > 2;
    } );

    $( "div.cardDetail label" ).hide();

    if ( $( "#HasResults" ).val() == "false" )
    {
        $( "#OtherSearchResults" ).hide();
    }

    $("#useAccountLocation").click(function() {
        $("#altSearchAddress").val("");
    });

    $("#toggleDistance").click(function() {

        if ($("#searchTools").is(":visible")) {
            $("#search").focus();
            $("#altSearchAddress").val("");
            $("input[type=radio]").prop("checked", false);
        }
    });
    
    $("#filter").change(function () {
        if ($(this).val() == "tags") {
            $("input#filterVal").hide();
            $("input#filterValTag").show();
        } else {
            $("input#filterVal").show();
            $("input#filterValTag").hide();
        }
    });

    $("#clearFilter").click(function() {
        $("#filterVal").val("");
        doFilter("");
    });
    function doFilter(val) {
        if (val.length == 0) {
            $("div.searchResult").show();
        } else {

            var filter = $("#filter").val().trim();

            $("div.searchResult").hide();

            var selector = "div.searchResult[" + filter + "*='" + val + "']";
            $(selector).show();
        }
    }

    $("input#filterVal,input#filterValTag").keyup(function () {
        var val = $(this).val().toLowerCase().trim();
        doFilter(val);
    });
    
    $(document).on("mouseover mouseout", 'div.searchResult, td.searchResult,  div.cardDetail', function () {
        if ( event.type == 'mouseover' )
        {
            $( this ).parent().find( ".resultHeader" ).show();
        } else
        {
            $( this ).parent().find( ".resultHeader" ).hide();
        }
    } );

    $(document).on("click", '.searchResult img', function () {
        var cId = $(this).parent().parent().attr("cardid");
        var orientation = $(this).attr("class") == "h_preview" ? "H" : "V";
        var thisImg = $(this).attr("src");
        var inMyBusidex = $(this).attr("inmybusidex");
        
        $("#detailPopup" + orientation).find("img.popupImg")
            .attr("src", thisImg)
            .attr("inmybusidex", inMyBusidex)
            .addClass($(this).attr("class"));

        $(".detailLink").each(function() {
            $(this).attr("href", "Details/" + cId);
        });
        
        if ($(this).attr("inmybusidex") == "true") {
            $("a.addToMyBusidex").attr("href","../Busidex/Mine#" + cId).html("View in MyBusidex");
        } else {
            $("a.addToMyBusidex").attr("href", "#").html("Add this card to MyBusidex").show()
                .unbind("click")               
                .click(function () {
                    return inMyBusidex == 'true' ? NavToMyBusidex() : AddToMyBusidex(cId);                                     
                });
        }         
    });

    $.fx.speeds._default = 900;
    
    $(document).on("click", 'div.resultHeader', function () {
        if ( !$( this ).hasClass( "added" ) )
        {
            $( this ).addClass( "added" );
            var cardId = $( this ).parent().attr( "cardId" );
            $.ajax( {
                type: 'POST',
                url: 'AddToMyBusidex',
                data: { id: cardId },
                success: function ()
                {
                    $( this ).parent().find( ".check" ).show();
                    alert( "Card added" );
                },
                error: function ()
                {
                    alert( 'There was a problem adding this card to your Busidex' );
                }
            } );
        }
        return false;
    } );

    $(document).on("click", 'span.addToBusidex', function () {
        if ( !$( this ).hasClass( "added" ) )
        {
            $( this ).addClass( "added" );
            var cardId = $( this ).attr( "cardId" );

            $.ajax( {
                type: 'POST',
                url: 'AddToMyBusidex',
                data: { id: cardId },
                success: function ()
                {
                    $( "img.check[cardId=" + cardId + "]" ).show();
                    $( "img.check[cardId=" + cardId + "]" ).next().text( "Card added" );
                },
                error: function ()
                {
                    alert( 'There was a problem adding this card to your Busidex' );
                }
            } );
        }
        return false;
    } );
});


function AddToMyBusidex(cId) {
    $.ajax({
        url: 'AddToMyBusidex',
        type: 'POST',
        data: { id: cId },
        success: function () {
            alert("This card is now in your Busidex!");
            $("a.addToMyBusidex").unbind("click").attr("href", "../Busidex/Mine#" + cId).html("View in MyBusidex");
        }
    });
}

function NavToMyBusidex(cId) {
    window.location.href = "../Busidex/Mine#" + cId;
    return true;
}
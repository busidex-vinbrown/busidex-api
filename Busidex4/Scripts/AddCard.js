
$( document ).ready( function ()
{
    $("#Add a").css('color', '#fff');
    
    $("div.ownerHelp").dialog({
        autoOpen:false,
        modal: true,
        height: 250,
        width: 400,
        resizable: false,
        title: 'Card Owner'
    });

    $(".ownerRequired").hide();
    if ($("#IsMyCard").is(":checked"))
    {
        $(".ownerRequired").show();
    }
    $("#IsMyCard").click(function () {
        if ( $("#Email").val().length == 0)
        {
            var email = $("#myEmail").val();
            $("#Email").val(email);
        }
        $(".ownerRequired").show();
    });

    $("#NotMyCard").click(function() {
        $(".ownerRequired").hide();
    });
    
    $("img.help").click(function() {
        $("div.ownerHelp").dialog('open');
    });
    
    setStepTitle( 'details' );
    
    $( ".relCardView" ).click( function ()
    {
        $( this ).parent().find( ".relSelected" ).toggle();
        var cardId = $( this ).attr( "cardId" );
        var $hdnRelSelected = $( "#hdnRelSelected_" + cardId );
        if ( $hdnRelSelected.val() == "" )
        {
            $hdnRelSelected.val( cardId );
        } else
        {
            $hdnRelSelected.val( "" );
        }
    } );

    var dialog = $( ".validation-summary-errors" ).dialog( {
        title: "Errors",
        resizable: false,
        modal: true,
        width: '400px',
        autoOpen: false
    } );

    dialog.dialog( 'open' );

    $( "#imgFrontReset" ).attr( "src", $( "#frontPreview" ).attr( "src" ) );
    $( "#imgBackReset" ).attr( "src", $( "#backPreview" ).attr( "src" ) );

    $( ".SizeWarning" ).click( function ()
    {
        $( "#fileSizeInfoMessage" ).show();
    } );

    $( ".ClosePopup" ).click( function ()
    {
        $( this ).parent().hide();
    } );

    $( "#o_fv" ).click( function ()
    {
        $( ".frontPreview" ).removeClass( "h_preview" ).addClass( "v_preview" );
    } );
    $( "#o_fh" ).click( function ()
    {
        $( ".frontPreview" ).removeClass( "v_preview" ).addClass( "h_preview" );
    } );
    $( "#o_bv" ).click( function ()
    {
        $( ".backPreview" ).removeClass( "h_preview" ).addClass( "v_preview" );
    } );
    $( "#o_bh" ).click( function ()
    {
        $( ".backPreview" ).removeClass( "v_preview" ).addClass( "h_preview" );
    } );

   $( ".link#frontReset" ).click( function ()
    {
        $( "#getFrontImgHdn" ).val( "" );
        $( ".SizeWarning" ).hide();
        $("#frontPreview,#frontPreview1,#frontPreview2,#frontPreview3,#frontPreview4").attr("src", $("#imgFrontReset").attr("src"));
    } );

    $( ".link#backReset" ).click( function ()
    {
        $( "#getBackImgHdn" ).val( "" );
        $( "#backPreview" ).attr( "src", $( "#imgBackReset" ).attr( "src" ) );
        $("#backPreview,#backPreview1,#backPreview2,#backPreview3,#frontPreview4").attr("src", $("#imgBackReset").attr("src"));
    } );

    $( "ul#AddCardSidebar li.step:first" ).addClass( "selected" );
    $( ".workArea:first" ).fadeIn();

    $('.alert').hide();
    $(".close").click(function() {
        $('.alert').hide();
    });
    $("input.submitLink").click(function () {
        $(this).hide();
        if ($("#IsMyCard").is(":checked")) {// only required for card owner
        
            if (!$("#cardForm :input[name=IsMyCard]").is(":checked")) {
                $('.alert #errorMessage').html('Please indicate if you are the card owner or not.');
                $('.alert').show();
                showStep("details");
                $(this).show();
                return false;
            }
            
            if ($("#cardForm :input[name=Name]").val().length == 0) {
                $('.alert #errorMessage').html("Please enter the name of the owner for this card.");
                $('.alert').show();
                showStep("details");
                $(this).show();
                return false;
            }
        
            if ($("#cardForm :input[name=Email]").val().length == 0) {
                $('.alert #errorMessage').html("Please enter an email for this card.");
                $('.alert').show();
                showStep("details");
                $(this).show();
                return false;
            }

            if ($("#cardForm :input[name=Number]").val().length == 0) {
                $('.alert #errorMessage').html("Please enter a phone number for this card.");
                $('.alert').show();
                showStep("details");
                $(this).show();
                return false;
            }
        }

        var hasFrontImage = $("#HasFrontImage").val();
        if ($("#cardForm :input[name=CardFrontImage]").val().length == 0 && hasFrontImage != "True" && $("#Display").val() == "IMG") {
            $('.alert #errorMessage').html("Please select an image to upload for this card.");
            $('.alert').show();
            showStep("details");
            $(this).show();
            return false;
        }

        try {            
            $("form#cardForm").submit();
        } catch (ex) {
            $('.alert #errorMessage').html(ex);
            $('.alert').show();
            $(this).show();
        }
        return true;
    } );

    $( "ul#AddCardSidebar li, input, img" ).click( function ()
    {
        if ( $( this ).hasClass( "step" ) )
        {
            $('input').removeClass('selected');
            $(this).addClass('selected');
            showStep($(this).attr("target"));
            
        }
    } );

    $( "#quickfind, .qfLabel" ).hide();

    $(document).on("click", 'a.removePhone', function () {
        var target = $( this ).attr("target");
        $( "#" + target ).hide();
        $(this).parent().find(".PhoneDeleted").val(true);
        $("div.phoneDiv:visible").last().find("a.addPhone").show();
        return false;
    } );

    /*PHONE NUMBERS*/
    $( "a.removePhone" ).eq( 0 ).hide();
    
    $(document).on("click", 'a.addPhone', function () {
        
        var template = $( "div.phoneDiv:visible" ).last();
        var newPhone = $( template ).clone();
        $(newPhone).find("input[type=text],input[type=tel],input[type=hidden]").each(function () {
            $( this ).val( "" );
        } );
        
        var newId = "cardPhone" + $( "div.phoneDiv" ).length;
       
        $("a.addPhone").hide();
        
        $(newPhone).insertAfter(template)
            .attr("id", newId)
            .find("a.removePhone")
            .show()
            .attr("target", newId);


        $("div.phoneDiv:visible").last().attr("id");
        
        return false;

    } );

    $("div.phoneDiv").eq(0).show();

    /*ADDRESSES*/
    $("#addNewAddress").click(function() {

        // get either the selected row or clone a new row
        var newAddress = $("tr.selected").length > 0 ? $("tr.selected") : $("tr#addressTemplate").clone();
        
        var address1 = $("#Address1").val();
        var address2 = $("#Address2").val();
        var city = $("#City").val();
        var state = $("#State").val();
        var zip = $("#ZipCode").val();
        var region = $("#Region").val();
        var country = $("#Country").val();
        
        $(newAddress).find(".Address1").val(address1);
        $(newAddress).find(".Address2").val(address2);
        $(newAddress).find(".City").val(city);
        $(newAddress).find(".State").val(state);
        $(newAddress).find(".ZipCode").val(zip);
        $(newAddress).find(".Region").val(region);
        $(newAddress).find(".Country").val(country);
        $(newAddress).find(".addressDisplay").html(address1 + " " + address2 + " " + city + " " + state + " " + zip + " " + region + " " + country);

        $("#addressList").append($(newAddress).fadeIn().removeAttr("id"));
        $(".addressField").val('');
        
        $("tr.addressRow").removeClass('selected');

    });

    $(document).on("click", ".removeAddress img", function () {
        var thisRow = $(this).parent().parent();
        $(thisRow).toggleClass('deleted');
        $(thisRow).find(".Deleted").val($(thisRow).hasClass('deleted') ? 'True' : 'False');
        
        var src = $(thisRow).hasClass('deleted') ? '/Images/undoDelete.png' : '/Images/delete.png';
        $(this).attr('src', src);
    });
    
    $(document).on("click", ".editAddress img", function () {
        $("tr.addressRow").removeClass('selected');
        var thisRow = $(this).parent().parent();
        $(thisRow).addClass('selected');
        var address1 = $(thisRow).find(".Address1").val();
        var address2 = $(thisRow).find(".Address2").val();
        var city = $(thisRow).find(".City").val();
        var state = $(thisRow).find(".State").val();
        var zip = $(thisRow).find(".ZipCode").val();
        var region = $(thisRow).find(".Region").val();
        var country = $(thisRow).find(".Country").val();
        
        $("#Address1").val(address1);
        $("#Address2").val(address2);
        $("#City").val(city);
        $("#State").val(state);
        $("#ZipCode").val(zip);
        $("#Region").val(region);
        $("#Country").val(country);
        
    });

    $("tr.addressRow").eq(1).addClass('selected');
    
    $("#getFrontImgHdn, #getBackImgHdn").change(function () {

        var form = $("#cardForm");
        var target = $(this).attr("target");
        $("#showImagePreview").val(true);
        
        var data = { idx: (target == 'backPreview' ? 1 : 0) };
        
        try {
            form.ajaxSubmit({
                data: data,
                cache:false,
                success: function (img) {
                    var startIdx = Modernizr.cssgradients ? 0 : 5;
                    var cleanImg = img.substr(startIdx).replace('</pre>', '');
                    cleanImg = cleanImg.replace('</PRE>', '');
                    $("." + target).attr("src", "data:image/gif;base64," + cleanImg);
                },
                complete: function() {
                    $("#showImagePreview").val(false);
                }
            });
        } catch(e) {
            alert(e);
        }
    });

    $("#newTag").keydown(function (e) {
        if (e.which == 13) {
            AddTag();
        }
    });
    $("#btnAddTag").click(function() {
        AddTag();
    });
});

$(document).on("click", 'span.removeTag', function () {
    $(this).parent().parent().fadeOut('fast', function () {
        $(this).remove();
    });
    $("#newTag").focus();
});

function AddTag() {
    var tagName = $("#newTag").val();
    if (tagName.length > 0) {
        var newTag = $("li#tagTemplate").clone().attr("id", "tag" + $(".tag").length).addClass("tag");
        newTag.find(".tag").val(tagName);
        newTag.find(".tagName").html(tagName);
        $("ul#tagList").append(newTag);
        $("ul#tagList li.tag").show();
        $("#newTag").val('');
    }
}

function showStep(target) {
    $(".workArea").hide();
    $("#" + target).show();
    setStepTitle(target);
}

function setStepTitle(target) {

    var t = '';
    
    switch ( target ) {        
        case 'details':
            {
                t = 'Add/Edit a Business Card';
                break;
            }
        case 'tags':
            {
                t = 'Tags';
                break;
            }
        case 'addresses':
            {
                t = 'Addresses';
                break;
            }
        case 'notes':
            {
                t = 'Add some notes';
                break;
            }
        case 'relations':
            {
                t = 'Relate this card to others in your collection';
                break;
            }
    }
    $( "#AddEditStepTitle" ).text( t );
}
function onListPopulated() {

    var completionList = $find("AutoCompleteEx").get_completionList();
    completionList.style.width = 'auto';
}

function subStringTextBox() {
    var okdpCode = document.getElementById('TextBox1').value.split(' ')[0];
    document.getElementById('TextBox1').value = okdpCode;
}

function textBoxKeyPress() {
    if (event.keyCode == 13) {
        __doPostBack('AddOKDPButton', '');
    }
}

function ExportToExcel() {
    __doPostBack('ExportXLSButton', '');
}

function CreateMailerF() {
    __doPostBack('Button10', '');
}

function DocReady() {
    if ($.browser.msie) {
        $("html").addClass("ie");
        if ($.browser.version < 10) {
            $("html").addClass("ie-lt10");
        }

        if ($.browser.version < 9) {
            $("html").addClass("ie-lt9");
        }
    }

    if ($.browser.mozilla) {
        $("html").addClass("ff");
    }

    if ($.browser.opera) {
        $("html").addClass("oo");
    }

    var img = new Image();
    $(img).load(function () {
    }).error(function () {
        $("html").addClass("noImages");
    }).attr('src', '/Static/Common/img/1x1.gif');

    $(".navigationLevel1_link").click(function () {
        if ($(this).parent().next().length == 0) return false;
        $(this).parent().parent().find(".navigationLevel1_item").removeClass().addClass("navigationLevel1_item");
        $(this).parent().addClass("navigationLevel1_item__active");
        if ($(this).parent().prev().length > 0) {
            $(this).parent().prev().addClass("navigationLevel1_item__activeLeft");
        };
        if ($(this).parent().next().length > 0) {
            $(this).parent().next().addClass("navigationLevel1_item__activeRight");
        };
        return false;
    });

    $(".headingFilter_link").click(function () {
        $(this).parent().parent().find(".headingFilter_item").removeClass("headingFilter_item__active");
        $(this).parent().addClass("headingFilter_item__active");
        return false;
    });


    $(".button, .field input").each(function () {
        $(this).attr("tabIndex", $(".button, .field input").index(this) + 1);
    });

    $(".button").focusin(function () {
        if (!$(this).hasClass("-disabled") && !$(this).parent().hasClass("-disabled")) {
            $(this).addClass("-focus");
            $(this).parent().removeClass("field__unhover");
        }

    });
    $(".button").focusout(function () {
        $(this).removeClass("-focus");
    });

    $(".field input").focusin(function () {
        if (!$(this).parent().hasClass("-disabled"))
            $(this).parent().addClass("-focus");
    });
    $(".field input").focusout(function () {
        $(this).parent().removeClass("-focus");
    });

    $(".field").mouseenter(function () {
        if (!$(this).hasClass("-disabled")) $(this).removeClass("field__unhover");
    });
    $(".field").mouseleave(function () {
        if (!$(this).hasClass("-disabled")) {
            $(this).addClass("field__unhover");
            $(this).find(".button").removeClass("-focus");
        }
    });

    $(".oo .button").mousedown(function () {
        return false;
    });

    $(".button").bind("mousedown", function () {
        if (!$(this).hasClass("-disabled") && !$(this).parent().hasClass("-disabled"))
            $(this).addClass("-active");
    });

    $(".button").bind("mouseup mouseleave", function () {
        $(this).removeClass("-active");
        $(this).removeClass("-focus");
        $(this).blur();
    });

    $(".button__type__calendar").click(function () {
        if (!$(this).hasClass("-disabled")) {
            $(this).toggleClass("button__opened");
            $(document).trigger("calendarOpened");
        }
    });

    $(".button__type__dropdown").click(function () {
        if ($(this).hasClass("button__opened")) {
            $(this).removeClass("button__opened");
            $(document).trigger("dropdownClosed");
        } else {
            $(this).addClass("button__opened");
            $(document).trigger("dropdownOpened");
        }
    });

    $(".buttonGroup .button").bind("mouseup", function () {
        if (!$(this).parent().hasClass("-disabled")) {
            $(this).parent().find(".button").removeClass("-checked");
            $(this).addClass("-checked");
        }
    });

    $(".buttonGroup, .buttonSwitcher").each(function () {
        $(this).find(".button:first").addClass("button__first");
        $(this).find(".button:last").addClass("button__last");
    });

}

//var splitBalance = $('#SplitBalance');
var splitBalanceDisplay = $('#SplitBalanceDisplay');


$(document).ready(function () {

    //Initialize Select2 Elements
    $('.select2').select2();
    //Date picker
    $('#TransactionDate').datepicker({
        autoclose: true
    });

    var splitIndex = 0;
    $("#btnAddSplit").click(function () {

        if ($('#createTransFormId').valid()) {
            $.ajax({
                url: '/Transactions/Split',//'@Url.Action("Split", "Transactions")',
                type: 'get',
                data: { index: splitIndex },
                success: function (response) {
                    splitIndex = splitIndex + 1;
                    $("#tbody_splitBody").append(response);
                },
                error: function (error) {
                    Console.log(error);
                    alert(error);
                }
            });
        }

    });

    $("#btnSplit").click(function () {

        if (!$('#createTransFormId').valid())
            return;

        displayRemainingBalance();

        $("#divSingleTrans").hide();
        $("#div_splitBody").show();

        //add initial row
        $.ajax({
            url: "/Transactions/Split",//'@Url.Action("Split", "Transactions")',
            type: 'get',
            data: { index: splitIndex },
            success: function (response) {
                splitIndex = splitIndex + 1;
                $("#tbody_splitBody").append(response);
            },
            error: function (error) {
                alert(error);
            }
        });

    });


    $(document).on('click', '#tbody_splitBody .removeRow', function () {

        $(this).closest('tr').remove();
    });

});


function GetRemainingBalance() {

    var depValue = $('input[name="Deposit"]').val();
    var payValue = $('input[name="Payment"]').val();

    var remainingBal = 0.0;

    if (depValue != null && depValue > 0) {
        remainingBal = parseFloat(depValue);
    }
    else {
        remainingBal = parseFloat(payValue);
    }

    $('.splitLineAmount').each(function () {

        var amt = $(this).val() == "" ? 0 : $(this).val();
        remainingBal = (remainingBal - amt);
    });

    return remainingBal;
}

function displayRemainingBalance() {

    splitBalanceDisplay.val(GetRemainingBalance());
}

function PercentChange() {
    getPercentage("PercentField");
    displayRemainingBalance();

}

function splitAmountChange() {
    getPercentage("AmountField");
    displayRemainingBalance();
}

function getPercentage(caller) {

    var depValue = $('input[name="Deposit"]').val();
    var payValue = $('input[name="Payment"]').val();

    var remainingBal = 0.0;

    if (depValue != null && depValue > 0) {
        remainingBal = parseFloat(depValue);
    }
    else {
        remainingBal = parseFloat(payValue);
    }

    $('#tbody_splitBody tr').each(function (index, ele) {

        if (caller == "PercentField") {
            var percent = $('input.Percent', this).val();
            if (percent != null && percent != "") {
                var result = ((percent / 100) * remainingBal);
                $('input.splitLineAmount', this).val(result);
            }
        }

        if (caller == "AmountField") {
            var value = $('input.splitLineAmount', this).val();
            if (value != null && value != "") {
                var result = ((value / remainingBal) * 100);
                $('input.Percent', this).val(result);
            }
        }

    })

}

function validateSplits() {

    var rows = $("#tbody_splitBody").find('tr')
    $(rows.find("select")).each(function () {
        var missing = $(this).val() === ""

        $(this).toggleClass('error', missing);
        $(this).parent().find('p').remove();
        if (missing)
            $(this).parent().append('<p class="text-danger">this field is required</p>');

    });

    $(rows.find("input.splitLineAmount")).each(function () {
        var missing = $(this).val() === ""

        $(this).toggleClass('error', missing);
        $(this).parent().find('p').remove();
        if (missing)
            $(this).parent().append('<p class="text-danger">this field is required</p>');

    });


    return rows.find(".error").length == 0
};

$('#Payment').change(function () {
    $('#Deposit').val("");
    if ($('#tbody_splitBody tr').length > 0)
        displayRemainingBalance();
});

$('#Deposit').change(function () {
    $('#Payment').val("");
    if ($('#tbody_splitBody tr').length > 0)
        displayRemainingBalance();
});

$('#btnCancel').click(function () {
    var accountRegistryId = $('#AccountRegistryId').val();
    var payment = $('#Payment').val();
    var deposit = $('#Deposit').val();
    var payee = $('#Payee').val();
    var TransactionDate = $('#TransactionDate').val();
    window.location.href = '/Transactions/Create?accountRegistryId=' + accountRegistryId + "&transDate=" + TransactionDate + "&pAmout=" + payment + "&dAmount=" + deposit + "&payee=" + payee;
    return false;
});



$('#submit').click(function () {

    var isValid = true;
    var isSplit = $('#tbody_splitBody tr').length > 0;

    //validate itemz
    if (!validateSplits())
        isValid = false;


    if (isSplit) {
        //validate Balance
        splitBalanceDisplay.toggleClass('error', GetRemainingBalance() != 0);
        splitBalanceDisplay.parent().find('p').remove();
        if (GetRemainingBalance() != 0) {
            splitBalanceDisplay.parent().append('<p class="text-danger">Incorrect amount allocation</p>');
            isValid = false;
        }
    }

    if (!isValid)
        return;

    var list = [];
    if (isSplit) {// split transaction
        $('#tbody_splitBody tr').each(function (index, ele) {

            var splitItem = {
                SplitAccountId: $('select.SplitAccountId', this).val(),
                SplitAccountFundId: $('select.SplitAccountFundId', this).val(),
                SplitAmount: parseFloat($('input.splitLineAmount', this).val())
            }
            list.push(splitItem);
        })
    }
    else { //single transactions
        var accountEl = $('#AccountId');
        var fundEl = $('#AccountFundId');

        //validate account
        accountEl.parent().find('p').remove();
        if (accountEl.val() == null || accountEl.val() == "") {
            accountEl.parent().append('<p class="text-danger">Account field is required</p>');
            isValid = false;
        }


        //validate fund
        fundEl.parent().find('p').remove();
        if (fundEl.val() == null || fundEl.val() == "") {
            fundEl.parent().append('<p class="text-danger">Fund field is required</p>');
            isValid = false;
        }

        if (isValid) {

            var amount = $('#Payment').val() == null || $('#Payment').val() == "" ? $('#Deposit').val() : $('#Payment').val();
            var splitItem = {
                SplitAccountId: $('#AccountId').val(),
                SplitAccountFundId: $('#AccountFundId').val(),
                SplitAmount: parseFloat(amount)
            }
            list.push(splitItem);
        }
    }

    if (!$('#createTransFormId').valid())
        isValid = false;


    if (!isValid)
        return;

    var data = {
        TransactionDate: $('#TransactionDate').val().trim(),
        Payee: $('#Payee').val().trim(),
        AccountRegistryId: $('#AccountRegistryId').val().trim(),
        Comment: $('#Comment').val().trim(),
        Payment: $('#Payment').val().trim(),
        Deposit: $('#Deposit').val().trim(),
        Splits: list
    }

    $.ajax({
        type: 'POST',
        url: '/transactions/CreateWithSplit',
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (data) {
            if (data.status == 200) {
                var accountRegistryId = $('#AccountRegistryId').val();
                window.location.href = '/Transactions?accountRegistryId=' + accountRegistryId;
            }
            else {
                alert('Error status: ' + data.status);
            }
        },
        error: function (error) {
            console.log(error);

        }
    });
});

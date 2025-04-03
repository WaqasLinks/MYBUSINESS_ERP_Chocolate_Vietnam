//// Move calculateTotal to the global scope
//function calculateTotal(inputId, multiplier, totalId) {
//    const inputValue = parseFloat(document.getElementById(inputId).value) || 0;
//    const totalValue = inputValue * multiplier;
//    document.getElementById(totalId).value = totalValue.toFixed(2); // Format to 2 decimal places

//    // Update the grand total for VND, USD, or JPY
//    updateGrandTotal();
//}

//// Function to update the grand total for VND, USD, and JPY
//function updateGrandTotal() {
//    // Calculate VND Grand Total
//    let vndTotal = 0;
//    document.querySelectorAll('[id^="total"]').forEach(input => {
//        if (input.id.includes("Vnd")) {
//            vndTotal += parseFloat(input.value) || 0;
//        }
//    });
//    document.getElementById("totalVnd").value = vndTotal.toFixed(2);

//    // Calculate USD Grand Total
//    let usdTotal = 0;
//    document.querySelectorAll('[id^="total"]').forEach(input => {
//        if (input.id.includes("Dollars")) {
//            usdTotal += parseFloat(input.value) || 0;
//        }
//    });
//    document.getElementById("totalDollars").value = usdTotal.toFixed(2);

//    // Calculate JPY Grand Total
//    let jpyTotal = 0;
//    document.querySelectorAll('[id^="total"]').forEach(input => {
//        if (input.id.includes("Yens")) {
//            jpyTotal += parseFloat(input.value) || 0;
//        }
//    });
//    document.getElementById("totalYens").value = jpyTotal.toFixed(2);
//}

//// Handle Bank Deposit link click
//$(document).ready(function () {
//    $('#bankDepositLink').click(function (e) {
//        e.preventDefault(); // Prevent default link behavior
//        $('#storeOpeningPopup').modal('show'); // Show the modal
//    });

//    // Handle Money Input link click
//    $('#moneyInputLink').click(function (e) {
//        e.preventDefault(); // Prevent default link behavior
//        $('#storeOpeningPopup').modal('show'); // Show the modal
//    });

//    $("#openShops").click(function (e) {
//        e.preventDefault(); // Prevent default form submission

//        var storeData = {
//            Id: 0, // Assuming it's a new store, update if needed
//            StoreId: $("#storeId").val(), // Get StoreId from input field
//            OpeningBalance: $("#openingBalance").val(),
//            OpeningCurrencyDetail: $("#openingCurrencyDetail").val()
//        };

//        $.ajax({
//            url: '/Stores/OpenShop', // Replace 'YourController' with the actual controller name
//            type: 'POST',
//            contentType: 'application/json',
//            data: JSON.stringify(storeData),
//            success: function (response) {
//                if (response.Success) {
//                    alert(response.Message);
//                    // Optionally reload the page or redirect
//                    location.reload();
//                } else {
//                    alert("Error: " + response.Message);
//                }
//            },
//            error: function () {
//                alert("An error occurred while processing your request.");
//            }
//        });
//    });
//});
// Function to calculate individual total based on input value and multiplier
// Move calculateTotal to the global scope
function calculateTotal(inputId, multiplier, totalId) {
    const inputValue = parseFloat(document.getElementById(inputId).value) || 0;
    console.log(`Input Value for ${inputId}: ${inputValue}`);
    const totalValue = inputValue * multiplier;
    console.log(`Total Value for ${totalId}: ${totalValue}`);
    document.getElementById(totalId).value = totalValue.toFixed(2);

    // Update the grand total for VND, USD, or JPY
    updateGrandTotal();
}

// Function to update the grand total for VND, USD, and JPY
function updateGrandTotal() {
    console.log("Updating Grand Totals...");

    // Calculate VND Grand Total
    let vndTotal = 0;
    const vndFields = [
        'totalOneThsndVnd', 'totalTwoThsndVnd', 'totalFiveThsndVnd',
        'totalTenThsndVnd', 'totalTwentyThsndVnd', 'totalFiftyThsndVnd',
        'totalOneLacVnd', 'totalTwoLacVnd', 'totalFiveLacVnd'
    ];
    vndFields.forEach(id => {
        const value = parseFloat(document.getElementById(id).value) || 0;
        console.log(`Value for ${id}: ${value}`);
        vndTotal += value;
    });
    console.log(`Total VND: ${vndTotal}`);
    document.getElementById("totalVnd").value = vndTotal.toFixed(2);

    // Calculate USD Grand Total
    let usdTotal = 0;
    const usdFields = [
        'totalOneDollar', 'totalFiveDollars', 'totalTenDollars',
        'totalTwentyDollars', 'totalFiftyDollars', 'totalHundredDollars'
    ];
    usdFields.forEach(id => {
        const value = parseFloat(document.getElementById(id).value) || 0;
        console.log(`Value for ${id}: ${value}`);
        usdTotal += value;
    });
    console.log(`Total USD: ${usdTotal}`);
    document.getElementById("totalDollars").value = usdTotal.toFixed(2);

    // Calculate JPY Grand Total
    let jpyTotal = 0;
    const jpyFields = [
        'totalOneYen', 'totalFiveYens', 'totalTenYens',
        'totalFiftyYens', 'totalHundredYens', 'totalFivehundredYens',
        'totalOnethousandsYens', 'totalTwothousandsYens', 'totalFivethousandsYens',
        'totalTenthousandsYens'
    ];
    jpyFields.forEach(id => {
        const value = parseFloat(document.getElementById(id).value) || 0;
        console.log(`Value for ${id}: ${value}`);
        jpyTotal += value;
    });
    console.log(`Total JPY: ${jpyTotal}`);
    document.getElementById("totalYens").value = jpyTotal.toFixed(2);
}

// Handle Bank Deposit link click
$(document).ready(function () {
    $('#bankDepositLink').click(function (e) {
        e.preventDefault(); // Prevent default link behavior
        $('#storeOpeningPopup').modal('show'); // Show the modal
    });

    // Handle Money Input link click
    $('#moneyInputLink').click(function (e) {
        e.preventDefault(); // Prevent default link behavior
        $('#storeOpeningPopup').modal('show'); // Show the modal
    });

    $("#openShops").click(function (e) {
        e.preventDefault(); // Prevent default form submission

        var storeData = {
            Id: 0, // Assuming it's a new store, update if needed
            StoreId: $("#storeId").val(), // Get StoreId from input field
            OpeningBalance: $("#openingBalance").val(),
            OpeningCurrencyDetail: $("#openingCurrencyDetail").val()
        };

        $.ajax({
            url: '/Stores/BankDeposit', // Replace 'YourController' with the actual controller name
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(storeData),
            success: function (response) {
                if (response.Success) {
                    alert(response.Message);
                    // Optionally reload the page or redirect
                    window.location.href = '/Stores/PrintReport?storeId=' + storeDto.StoreId;
                } else {
                    alert("Error: " + response.Message);
                }
            },
            error: function () {
                alert("An error occurred while processing your request.");
            }
        });
    });
});
function PrintRDLCReport() {
    var reportViewer = document.getElementById('ReportViewer1');
    if (reportViewer) {
        reportViewer.contentWindow.print(); // Triggers print
    }
}
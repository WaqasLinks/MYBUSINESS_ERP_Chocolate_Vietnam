var serverDownError = "Server is down VAT Invoice cannot print at this time";
//var storeId = 0;
// Declare the active input variable
let activeInputClose = null;
var availableCurrenciesCloseStore = [];

//if (storeId != null) {
//    alert("Null" + storeId);
//}
//else {
//    alert("Exists" + storeId);
//}

$(document).ready(function () {
    debugger;
    getAllAvailableCurrenciesCloseStore();
});


function getAllAvailableCurrenciesCloseStore() {
    $.ajax({
        url: '/Currencies/GetAllAbvailableCurrencies',
        type: 'GET',
        success: function (response) {
            if (response.Success) {
                availableCurrenciesCloseStore = response.Data;
            } else {
                alert('Failed to set session: ' + response.Message);
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred while setting the session: ' + error);
        }
    });
}

//function getAllAvailableCurrenciesCloseStore() {
//    $.ajax({
//        url: '/Currencies/GetAllAbvailableCurrencies',
//        type: 'GET',
//        success: function (response) {
//            if (response.Success) {
//                availableCurrenciesCloseStore = response.Data;
//                /*var url = $('#closeStore').data('url');*/ // Get the URL here
//                /*showModalAndFocusInputClose*/(url); // Pass the URL to the function
//            } else {
//                alert('Failed to set session: ' + response.Message);
//            }
//        },
//        error: function (xhr, status, error) {
//            alert('An error occurred while setting the session: ' + error);
//        }
//    });
//}

//function showModalAndFocusInputClose(url) { // Accept URL as a parameter
//    $.get(url, function (data) {
//        $('body').append(data);
//        $('#storeClosingModal').modal('show');
//        $('#storeClosingModal').on('shown.bs.modal', function () {
//            var activeInputClose = document.getElementById('oneThsndVndClose');
//            if (activeInputClose) {
//                activeInputClose.focus(); // Set focus to the input
//            }
//        });
//        $('#storeClosingModal').on('hidden.bs.modal', function () {
//            $(this).remove();
//        });
//    });
//}

//// Use event delegation to ensure the event is captured
//$(document).on('click', '#closeStore', function () {
//    getAllAvailableCurrenciesCloseStore();
//});
// Attach click handler to elements with the 'storeids' class

$(document).on('click', '#closeStore', function (e) {
    e.preventDefault();
    $(this).attr('disabled', true); // Disable the link
    var url = $(this).data('url');
    $.get(url, function (data) {
        $('body').append(data);
        // Show the modal
        $('#storeClosingModal').modal('show');
        $('#closeStore').attr('disabled', false); // Re-enable the link if needed
        // Set default active input
        // Set default active input after a brief delay
        $('#storeClosingModal').on('shown.bs.modal', function () {
            activeInputClose = document.getElementById('oneThsndVndClose');
            if (activeInputClose) {
                activeInputClose.focus(); // Set focus to the input
            }
        });
        $('#storeClosingModal').on('hidden.bs.modal', function () {
            $(this).remove();
        });
    });
});
//Attach focus event listener only once
$(document).on('click', '#bankDeposits', function (e) {
    e.preventDefault();
    $(this).attr('disabled', true); // Disable the link
    var url = $(this).data('url');
    $.get(url, function (data) {
        $('body').append(data);
        // Show the modal
        $('#storeClosingModal').modal('show');
        $('#bankDeposits').attr('disabled', false); // Re-enable the link if needed
        // Set default active input
        // Set default active input after a brief delay
        $('#storeClosingModal').on('shown.bs.modal', function () {
            activeInputClose = document.getElementById('oneThsndVndClose');
            if (activeInputClose) {
                activeInputClose.focus(); // Set focus to the input
            }
        });
        $('#storeClosingModal').on('hidden.bs.modal', function () {
            $(this).remove();
        });
    });
});
$(document).on('click', '#moneyInputs', function (e) {
    e.preventDefault();
    $(this).attr('disabled', true); // Disable the link
    var url = $(this).data('url');
    $.get(url, function (data) {
        $('body').append(data);
        // Show the modal
        $('#storeClosingModal').modal('show');
        $('#moneyInputs').attr('disabled', false); // Re-enable the link if needed
        // Set default active input
        // Set default active input after a brief delay
        $('#storeClosingModal').on('shown.bs.modal', function () {
            activeInputClose = document.getElementById('oneThsndVndClose');
            if (activeInputClose) {
                activeInputClose.focus(); // Set focus to the input
            }
        });
        $('#storeClosingModal').on('hidden.bs.modal', function () {
            $(this).remove();
        });
    });
});
$(document).on('focus', 'input', function () {
    activeInputClose = this; // Update the active input when focused
});

// Insert number into active input
function insertNumberClose(number) {
    if (activeInputClose) {
        // If 'CE' is pressed, clear the input field
        if (number === 'CE') {
            activeInputClose.value = '0';
        }
        // If 'X' is pressed, remove the last character
        else if (number === 'X') {
            activeInputClose.value = activeInputClose.value.length === 1 ? '0' : activeInputClose.value.slice(0, -1);
        }
        // If the current value is '0', replace it with the new number
        else {
            if (activeInputClose.value === '0') {
                activeInputClose.value = number;
            } else {
                activeInputClose.value += number;
            }
        }
        // Update the total for the active input
        updateTotalForActiveInputClose();
        updateTotalForActiveInputCloseDollars();
        updateTotalForActiveInputCloseYens();
    }
}

// Update total for the active input
function updateTotalForActiveInputClose() {
    const denominationMap = {
        'oneThsndVndClose': 1000,
        'twoThsndVndClose': 2000,
        'fiveThsndVndClose': 5000,
        'tenThsndVndClose': 10000,
        'twentyThsndVndClose': 20000,
        'fiftyThsndVndClose': 50000,
        'oneLacVndClose': 100000,
        'twoLacVndClose': 200000,
        'fiveLacVndClose': 500000
    };

    const inputId = activeInputClose.id; // Get the ID of the active input field
    const denominationValue = denominationMap[inputId]; // Get the denomination value

    if (denominationValue) {
        const count = parseInt(activeInputClose.value) || 0;
        const total = count * denominationValue;
        document.getElementById(`total${inputId.charAt(0).toUpperCase() + inputId.slice(1)}`).value = total;
        updateOverallTotalsClose();
    }
}
//Dollars
function updateTotalForActiveInputCloseDollars() {
    const denominationMap = {
        'oneDollarClose': 1,
        'fiveDollarsClose': 5,
        'tenDollarsClose': 10,
        'twentyDollarsClose': 20,
        'fiftyDollarsClose': 50,
        'hundredDollarsClose': 100
    };

    const inputId = activeInputClose.id; // Get the ID of the active input field
    const denominationValue = denominationMap[inputId]; // Get the denomination value

    if (denominationValue) {
        const count = parseInt(activeInputClose.value) || 0;
        const total = count * denominationValue;
        document.getElementById(`total${inputId.charAt(0).toUpperCase() + inputId.slice(1)}`).value = total;
        updateOverallTotalsCloseDollars();
    }
}
//Yens / JPY
function updateTotalForActiveInputCloseYens() {
    const denominationMap = {
        'oneYenClose': 1,
        'fiveYensClose': 5,
        'tenYensClose': 10,
        'fiftyYensClose': 50,
        'hundredYensClose': 1000,
        'fivehundredYensClose': 500,
        'onethousandsYens': 1000,
        'twothousandsYensClose': 2000,
        'fivethousandsYensClose': 5000,
        'tenthousandsYensClose': 10000,
    };

    const inputId = activeInputClose.id; // Get the ID of the active input field
    const denominationValue = denominationMap[inputId]; // Get the denomination value

    if (denominationValue) {
        const count = parseInt(activeInputClose.value) || 0;
        const total = count * denominationValue;
        document.getElementById(`total${inputId.charAt(0).toUpperCase() + inputId.slice(1)}`).value = total;
        updateOverallTotalsCloseYens();
    }
}
$(document).on('click', '#closeShop', function () {
    // Get and validate input values
    var totalAmountVndClose = document.getElementById('totalVndCountClose').value;
    if (totalAmountVndClose == '0') totalAmountVndClose = 0;

    // Confirmation dialog
    if (!confirm(`You input '${totalAmountVndClose}', are you sure?`)) {
        alert("Operation cancelled.");
        $('#storeClosingPopup').modal('hide');
        $('#storeClosingPopup').find('input, textarea, select').val('0');
        return;
    }

    // Initialize counters
    var totalQuantity = 0;
    var vndQuantity = 0;
    var usdQuantity = 0;
    var jpyQuantity = 0;
    var currencyName = "VND"; // Default currency

    // 1. Count VND bills
    var vndInputs = [
        'oneThsndVndClose', 'twoThsndVndClose', 'fiveThsndVndClose',
        'tenThsndVndClose', 'twentyThsndVndClose', 'fiftyThsndVndClose',
        'oneLacVndClose', 'twoLacVndClose', 'fiveLacVndClose'
    ];

    vndInputs.forEach(function (inputId) {
        var count = parseInt($('#' + inputId).val()) || 0;
        vndQuantity += count;
        totalQuantity += count;
    });

    // 2. Count USD bills
    var usdInputs = [
        'oneDollarClose', 'fiveDollarsClose', 'tenDollarsClose',
        'twentyDollarsClose', 'fiftyDollarsClose', 'hundredDollarsClose'
    ];

    usdInputs.forEach(function (inputId) {
        var count = parseInt($('#' + inputId).val()) || 0;
        usdQuantity += count;
        totalQuantity += count;
    });

    // 3. Count JPY coins/bills
    var jpyInputs = [
        'oneYenClose', 'fiveYensClose', 'tenYensClose', 'fiftyYensClose',
        'hundredYensClose', 'fivehundredYensClose', 'onethousandsYensClose',
        'twothousandsYensClose', 'fivethousandsYensClose', 'tenthousandsYensClose'
    ];

    jpyInputs.forEach(function (inputId) {
        var count = parseInt($('#' + inputId).val()) || 0;
        jpyQuantity += count;
        totalQuantity += count;
    });

    // 4. Format currency details string
    var formattedString = "";
    if (closeCurrencyDetalInVnd.length > 0) {
        formattedString = closeCurrencyDetalInVnd.map(item => {
            let denominationValue = 0;
            switch (item.denomination) {
                case 'oneThsndVndClose': denominationValue = 1000; break;
                case 'twoThsndVndClose': denominationValue = 2000; break;
                case 'fiveThsndVndClose': denominationValue = 5000; break;
                case 'tenThsndVndClose': denominationValue = 10000; break;
                case 'twentyThsndVndClose': denominationValue = 20000; break;
                case 'fiftyThsndVndClose': denominationValue = 50000; break;
                case 'oneLacVndClose': denominationValue = 100000; break;
                case 'twoLacVndClose': denominationValue = 200000; break;
                case 'fiveLacVndClose': denominationValue = 500000; break;
                default: denominationValue = 0;
            }
            return `${denominationValue}@${item.count}`;
        }).join(':');
    }

    // 5. Determine which currency is being used
    var selectedBlance = 0;
    var vndBalance = parseFloat($('#totalVndCountClose').val()) || 0;
    var dollarBalance = parseFloat($('#totalDollarsCountClose').val()) || 0;
    var jpyBalance = parseFloat($('#totalYensCountClose').val()) || 0;

    if (vndBalance !== 0) {
        selectedBlance = vndBalance;
        currencyName = "VND";
    }
    else if (dollarBalance !== 0) {
        selectedBlance = dollarBalance;
        currencyName = "USD";
    }
    else if (jpyBalance !== 0) {
        selectedBlance = jpyBalance;
        currencyName = "JPY";
    }

    // 6. Prepare the view model
    var storeViewModel = {
        ClosingBalance: selectedBlance,
        ClosingCurrencyDetail: formattedString,
        OpeningBalance: selectedBlance,
        Quantity: totalQuantity,
        VNDQuantity: vndQuantity,
        USDQuantity: usdQuantity,
        JPYQuantity: jpyQuantity,
        CurrencyName: currencyName
    };

    // 7. Send data to server
    $.ajax({
        url: '/Stores/CloseShop',
        type: 'POST',
        data: JSON.stringify({
            StoreId: parseInt(localStorage.getItem('currentStoreId')), // Get from storage
            ClosingBalance: storeViewModel.ClosingBalance,
            ClosingCurrencyDetail: storeViewModel.ClosingCurrencyDetail
            // include other needed properties
        }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            if (response.Success) {
                window.location.href = '/Stores/StoreDashboard';
                localStorage.removeItem('storeIds');
            } else {
                alert('Error: ' + response.Message);
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred: ' + error);
        }
    });
});
$(document).on('click', '#bankDeposit', function () {
    var totalAmountVndClose = document.getElementById('totalVndCountClose').value;
    if (totalAmountVndClose == '0') totalAmountVndClose = 0;

    if (!confirm(`You input '${totalAmountVndClose}', are you sure?`)) {
        alert("Operation cancelled.");
        $('#storeClosingPopup').modal('hide');
        $('#storeClosingPopup').find('input, textarea, select').val('0');
        return;
    }

    // Initialize counters
    var totalQuantity = 0, vndQuantity = 0, usdQuantity = 0, jpyQuantity = 0, currencyName = "VND";

    const getCount = id => parseInt($('#' + id).val()) || 0;

    const vndInputs = ['oneThsndVndClose', 'twoThsndVndClose', 'fiveThsndVndClose',
        'tenThsndVndClose', 'twentyThsndVndClose', 'fiftyThsndVndClose',
        'oneLacVndClose', 'twoLacVndClose', 'fiveLacVndClose'];

    vndInputs.forEach(id => { let c = getCount(id); vndQuantity += c; totalQuantity += c; });

    const usdInputs = ['oneDollarClose', 'fiveDollarsClose', 'tenDollarsClose',
        'twentyDollarsClose', 'fiftyDollarsClose', 'hundredDollarsClose'];

    usdInputs.forEach(id => { let c = getCount(id); usdQuantity += c; totalQuantity += c; });

    const jpyInputs = ['oneYenClose', 'fiveYensClose', 'tenYensClose', 'fiftyYensClose',
        'hundredYensClose', 'fivehundredYensClose', 'onethousandsYensClose',
        'twothousandsYensClose', 'fivethousandsYensClose', 'tenthousandsYensClose'];

    jpyInputs.forEach(id => { let c = getCount(id); jpyQuantity += c; totalQuantity += c; });

    // Select balance and currency
    var vndBalance = parseFloat($('#totalVndCountClose').val()) || 0;
    var dollarBalance = parseFloat($('#totalDollarsCountClose').val()) || 0;
    var jpyBalance = parseFloat($('#totalYensCountClose').val()) || 0;

    var selectedBalance = 0;
    if (vndBalance !== 0) {
        selectedBalance = vndBalance;
        currencyName = "VND";
    } else if (dollarBalance !== 0) {
        selectedBalance = dollarBalance;
        currencyName = "USD";
    } else if (jpyBalance !== 0) {
        selectedBalance = jpyBalance;
        currencyName = "JPY";
    }

    var shopManagementViewModel = {
        Balance: selectedBalance,
        Quantity: totalQuantity,
        VNDQuantity: vndQuantity,
        USDQuantity: usdQuantity,
        JPYQuantity: jpyQuantity,
        CurrencyName: currencyName
    };

    $.ajax({
        url: '/Stores/BankDeposit',
        type: 'POST',
        data: JSON.stringify(shopManagementViewModel),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            if (response.Success) {
                const reportUrl = `/Stores/GenerateBankDepositReport?bankDepositId=${encodeURIComponent(response.bankDepositId)}`;
                window.open(reportUrl, '_blank');
            } else {
                alert('Error: ' + response.Message);
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred: ' + error);
        }
    });
});

//$(document).on('click', '#bankDeposit', function () {
//    /*alert('bankDeposit')*/
//    var totalAmountVndClose = document.getElementById('totalVndCountClose').value;
//    if (totalAmountVndClose == '0') totalAmountVndClose = 0;
//    if (confirm(`You input '${totalAmountVndClose}', are you sure?`)) {
//        // Confirmation dialog
//        if (!confirm(`You input '${totalAmountVndClose}', are you sure?`)) {
//            alert("Operation cancelled.");
//            $('#storeClosingPopup').modal('hide');
//            $('#storeClosingPopup').find('input, textarea, select').val('0');
//            return;
//        }

//        // Initialize counters
//        var totalQuantity = 0;
//        var vndQuantity = 0;
//        var usdQuantity = 0;
//        var jpyQuantity = 0;
//        var currencyName = "VND"; // Default currency

//        // 1. Count VND bills
//        var vndInputs = [
//            'oneThsndVndClose', 'twoThsndVndClose', 'fiveThsndVndClose',
//            'tenThsndVndClose', 'twentyThsndVndClose', 'fiftyThsndVndClose',
//            'oneLacVndClose', 'twoLacVndClose', 'fiveLacVndClose'
//        ];

//        vndInputs.forEach(function (inputId) {
//            var count = parseInt($('#' + inputId).val()) || 0;
//            vndQuantity += count;
//            totalQuantity += count;
//        });

//        // 2. Count USD bills
//        var usdInputs = [
//            'oneDollarClose', 'fiveDollarsClose', 'tenDollarsClose',
//            'twentyDollarsClose', 'fiftyDollarsClose', 'hundredDollarsClose'
//        ];

//        usdInputs.forEach(function (inputId) {
//            var count = parseInt($('#' + inputId).val()) || 0;
//            usdQuantity += count;
//            totalQuantity += count;
//        });

//        // 3. Count JPY coins/bills
//        var jpyInputs = [
//            'oneYenClose', 'fiveYensClose', 'tenYensClose', 'fiftyYensClose',
//            'hundredYensClose', 'fivehundredYensClose', 'onethousandsYensClose',
//            'twothousandsYensClose', 'fivethousandsYensClose', 'tenthousandsYensClose'
//        ];

//        jpyInputs.forEach(function (inputId) {
//            var count = parseInt($('#' + inputId).val()) || 0;
//            jpyQuantity += count;
//            totalQuantity += count;
//        });

//        // 4. Format currency details string
//        var formattedString = "";
//        if (closeCurrencyDetalInVnd.length > 0) {
//            formattedString = closeCurrencyDetalInVnd.map(item => {
//                let denominationValue = 0;

//                switch (item.denomination) {
//                    case 'oneThsndVndClose':
//                        denominationValue = 1000;
//                        break;
//                    case 'twoThsndVndClose':
//                        denominationValue = 2000;
//                        break;
//                    case 'fiveThsndVndClose':
//                        denominationValue = 5000;
//                        break;
//                    case 'tenThsndVndClose':
//                        denominationValue = 10000;
//                        break;
//                    case 'twentyThsndVndClose':
//                        denominationValue = 20000;
//                        break;
//                    case 'fiftyThsndVndClose':
//                        denominationValue = 50000;
//                        break;
//                    case 'oneLacVndClose':
//                        denominationValue = 100000;
//                        break;
//                    case 'twoLacVndClose':
//                        denominationValue = 200000;
//                        break;
//                    case 'fiveLacVndClose':
//                        denominationValue = 500000;
//                        break;
//                    default:
//                        denominationValue = 0;
//                }
//                return `${denominationValue}@${item.count}`;
//            }).join(':');
//        }
//        var selectedBlance = 0;
//        var vndBalance = parseFloat($('#totalVndCountClose').val());
//        var dollarBalance = parseFloat($('#totalDollarsCountClose').val());
//        var jpyBalance = parseFloat($('#totalYensCountClose').val());

//        if (!isNaN(vndBalance) && vndBalance !== 0) {
//            selectedBlance = vndBalance;
//            currencyName = "VND";
//        } else if (!isNaN(dollarBalance) && dollarBalance !== 0) {
//            selectedBlance = dollarBalance;
//            currencyName = "USD";
//        } else if (!isNaN(jpyBalance) && jpyBalance !== 0) {
//            selectedBlance = jpyBalance;
//            currencyName = "JPY";
//        } else {
//            vndBalance = 0;
//            dollarBalance = 0;
//            jpyBalance = 0;
//        }

//        var ShopManagementViewModel = {
//            Balance: selectedBlance,
//            //ClosingCurrencyDetail: formattedString,
//            //OpeningBalance: selectedBlance,

//            Quantity: totalQuantity,
//            VNDQuantity: vndQuantity,
//            USDQuantity: usdQuantity,
//            JPYQuantity: jpyQuantity,
//            CurrencyName: currencyName
//        };
//        //$.ajax({
//        //    url: '/Stores/BankDeposit',
//        //    type: 'POST',
//        //    data: JSON.stringify(storeViewModel),
//        //    contentType: 'application/json; charset=utf-8',
//        //    xhrFields: {
//        //        responseType: 'blob'  // Expect a binary file (PDF)
//        //    },
//        //    success: function (response, status, xhr) {
//        //        // Create a Blob object from the response
//        //        var blob = new Blob([response], { type: 'application/pdf' });

//        //        // Get filename from content-disposition header if present
//        //        var filename = "BankDepositReport.pdf";
//        //        var contentDisposition = xhr.getResponseHeader('Content-Disposition');
//        //        if (contentDisposition) {
//        //            var matches = contentDisposition.match(/filename="(.+)"/);
//        //            if (matches.length === 2) filename = matches[1];
//        //        }

//        //        // Create a link to download the file
//        //        var link = document.createElement('a');
//        //        link.href = window.URL.createObjectURL(blob);
//        //        link.download = filename;
//        //        document.body.appendChild(link);
//        //        link.click();
//        //        document.body.removeChild(link);

//        //        // Redirect after download (optional)
//        //        window.location.href = '/Stores/StoreDashboard';
//        //        localStorage.removeItem('storeIds');
//        //    },
//        //    error: function (xhr, status, error) {
//        //        alert('An error occurred: ' + error);
//        //    }
//        //});


//        $.ajax({
//            url: '/Stores/BankDeposit',
//            type: 'POST',
//            data: JSON.stringify(ShopManagementViewModel),
//            contentType: 'application/json; charset=utf-8',
//            dataType: 'json',
//            success: function (response) {
//                debugger

//                if (response.Success) {
//                    window.open('/Stores/GenerateBankDepositReport?bankDepositId=' + response.bankDepositId, '_blank');
//                    //$.ajax({
//                    //    url: '/Stores/GenerateBankDepositReport',
//                    //    type: 'GET',
//                    //    data: response.bankDepositId,
                        
//                    //    xhrFields: {
//                    //        responseType: 'blob' // Get response as Blob
//                    //    },
//                    //    success: function (response) {
//                    //        // Create a Blob from the PDF response
//                    //        var blob = new Blob([response], { type: 'application/pdf' });

//                    //        // Create an object URL for the Blob
//                    //        var blobUrl = URL.createObjectURL(blob);

//                    //        // Open the PDF in a new window
//                    //        var newTab = window.open(blobUrl, '_blank');

//                    //        // Ensure the tab is opened before triggering print
//                    //        if (newTab) {
//                    //            newTab.onload = function () {
//                    //                newTab.print();
//                    //            };
//                    //        } else {
//                    //            alert('Popup blocked! Please allow popups for this website.');
//                    //        }
//                    //    },
//                    //    error: function (xhr, status, error) {
//                    //        alert('An error occurred: ' + error);
//                    //    }
//                    //});
//                } else {
//                    alert('Error: ' + response.Message);
//                }
//            },
//            error: function (xhr, status, error) {
//                alert('An error occurred: ' + error);
//            }
//        });


//    } else {
//        alert("Operation cancelled.");
//        $('#storeClosingPopup').modal('hide');
//        $('#storeClosingPopup').find('input, textarea, select').val('0');
//    }
//});
$(document).on('click', '#moneyInput', function () {
    /*alert('bankDeposit')*/
    var totalAmountVndClose = document.getElementById('totalVndCountClose').value;
    if (totalAmountVndClose == '0') totalAmountVndClose = 0;
    if (confirm(`You input '${totalAmountVndClose}', are you sure?`)) {
        // Confirmation dialog
        if (!confirm(`You input '${totalAmountVndClose}', are you sure?`)) {
            alert("Operation cancelled.");
            $('#storeClosingPopup').modal('hide');
            $('#storeClosingPopup').find('input, textarea, select').val('0');
            return;
        }

        // Initialize counters
        var totalQuantity = 0;
        var vndQuantity = 0;
        var usdQuantity = 0;
        var jpyQuantity = 0;
        var currencyName = "VND"; // Default currency

        // 1. Count VND bills
        var vndInputs = [
            'oneThsndVndClose', 'twoThsndVndClose', 'fiveThsndVndClose',
            'tenThsndVndClose', 'twentyThsndVndClose', 'fiftyThsndVndClose',
            'oneLacVndClose', 'twoLacVndClose', 'fiveLacVndClose'
        ];

        vndInputs.forEach(function (inputId) {
            var count = parseInt($('#' + inputId).val()) || 0;
            vndQuantity += count;
            totalQuantity += count;
        });

        // 2. Count USD bills
        var usdInputs = [
            'oneDollarClose', 'fiveDollarsClose', 'tenDollarsClose',
            'twentyDollarsClose', 'fiftyDollarsClose', 'hundredDollarsClose'
        ];

        usdInputs.forEach(function (inputId) {
            var count = parseInt($('#' + inputId).val()) || 0;
            usdQuantity += count;
            totalQuantity += count;
        });

        // 3. Count JPY coins/bills
        var jpyInputs = [
            'oneYenClose', 'fiveYensClose', 'tenYensClose', 'fiftyYensClose',
            'hundredYensClose', 'fivehundredYensClose', 'onethousandsYensClose',
            'twothousandsYensClose', 'fivethousandsYensClose', 'tenthousandsYensClose'
        ];

        jpyInputs.forEach(function (inputId) {
            var count = parseInt($('#' + inputId).val()) || 0;
            jpyQuantity += count;
            totalQuantity += count;
        });

        // 4. Format currency details string
        var formattedString = "";
        if (closeCurrencyDetalInVnd.length > 0) {
            formattedString = closeCurrencyDetalInVnd.map(item => {
                let denominationValue = 0;

                switch (item.denomination) {
                    case 'oneThsndVndClose':
                        denominationValue = 1000;
                        break;
                    case 'twoThsndVndClose':
                        denominationValue = 2000;
                        break;
                    case 'fiveThsndVndClose':
                        denominationValue = 5000;
                        break;
                    case 'tenThsndVndClose':
                        denominationValue = 10000;
                        break;
                    case 'twentyThsndVndClose':
                        denominationValue = 20000;
                        break;
                    case 'fiftyThsndVndClose':
                        denominationValue = 50000;
                        break;
                    case 'oneLacVndClose':
                        denominationValue = 100000;
                        break;
                    case 'twoLacVndClose':
                        denominationValue = 200000;
                        break;
                    case 'fiveLacVndClose':
                        denominationValue = 500000;
                        break;
                    default:
                        denominationValue = 0;
                }
                return `${denominationValue}@${item.count}`;
            }).join(':');
        }
        var selectedBlance = 0;
        var vndBalance = parseFloat($('#totalVndCountClose').val());
        var dollarBalance = parseFloat($('#totalDollarsCountClose').val());
        var jpyBalance = parseFloat($('#totalYensCountClose').val());

        if (!isNaN(vndBalance) && vndBalance !== 0) {
            selectedBlance = vndBalance;
            currencyName = "VND";
        } else if (!isNaN(dollarBalance) && dollarBalance !== 0) {
            selectedBlance = dollarBalance;
            currencyName = "USD";
        } else if (!isNaN(jpyBalance) && jpyBalance !== 0) {
            selectedBlance = jpyBalance;
            currencyName = "JPY";
        } else {
            vndBalance = 0;
            dollarBalance = 0;
            jpyBalance = 0;
        }

        var storeViewModel = {
            ClosingBalance: selectedBlance,
            ClosingCurrencyDetail: formattedString,
            OpeningBalance: selectedBlance,
            Quantity: totalQuantity,
            VNDQuantity: vndQuantity,
            USDQuantity: usdQuantity,
            JPYQuantity: jpyQuantity,
            CurrencyName: currencyName
        };
        //$.ajax({
        //    url: '/Stores/BankDeposit',
        //    type: 'POST',
        //    data: JSON.stringify(storeViewModel),
        //    contentType: 'application/json; charset=utf-8',
        //    xhrFields: {
        //        responseType: 'blob'  // Expect a binary file (PDF)
        //    },
        //    success: function (response, status, xhr) {
        //        // Create a Blob object from the response
        //        var blob = new Blob([response], { type: 'application/pdf' });

        //        // Get filename from content-disposition header if present
        //        var filename = "BankDepositReport.pdf";
        //        var contentDisposition = xhr.getResponseHeader('Content-Disposition');
        //        if (contentDisposition) {
        //            var matches = contentDisposition.match(/filename="(.+)"/);
        //            if (matches.length === 2) filename = matches[1];
        //        }

        //        // Create a link to download the file
        //        var link = document.createElement('a');
        //        link.href = window.URL.createObjectURL(blob);
        //        link.download = filename;
        //        document.body.appendChild(link);
        //        link.click();
        //        document.body.removeChild(link);

        //        // Redirect after download (optional)
        //        window.location.href = '/Stores/StoreDashboard';
        //        localStorage.removeItem('storeIds');
        //    },
        //    error: function (xhr, status, error) {
        //        alert('An error occurred: ' + error);
        //    }
        //});


        $.ajax({
            url: '/Stores/MoneyInput',
            type: 'POST',
            data: JSON.stringify(storeViewModel),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                debugger

                if (response.Success) {
                    window.open('/Stores/GenerateMoneyInputReport?bankDepositId=' + response.bankDepositId, '_blank');
                    //$.ajax({
                    //    url: '/Stores/GenerateBankDepositReport',
                    //    type: 'GET',
                    //    data: response.bankDepositId,

                    //    xhrFields: {
                    //        responseType: 'blob' // Get response as Blob
                    //    },
                    //    success: function (response) {
                    //        // Create a Blob from the PDF response
                    //        var blob = new Blob([response], { type: 'application/pdf' });

                    //        // Create an object URL for the Blob
                    //        var blobUrl = URL.createObjectURL(blob);

                    //        // Open the PDF in a new window
                    //        var newTab = window.open(blobUrl, '_blank');

                    //        // Ensure the tab is opened before triggering print
                    //        if (newTab) {
                    //            newTab.onload = function () {
                    //                newTab.print();
                    //            };
                    //        } else {
                    //            alert('Popup blocked! Please allow popups for this website.');
                    //        }
                    //    },
                    //    error: function (xhr, status, error) {
                    //        alert('An error occurred: ' + error);
                    //    }
                    //});
                } else {
                    alert('Error: ' + response.Message);
                }
            },
            error: function (xhr, status, error) {
                alert('An error occurred: ' + error);
            }
        });


    } else {
        alert("Operation cancelled.");
        $('#storeClosingPopup').modal('hide');
        $('#storeClosingPopup').find('input, textarea, select').val('0');
    }
});
function calculateTotalClose(inputId, denominationValue, outputId) {
    const count = parseInt(document.getElementById(inputId).value) || 0;
    const total = count * denominationValue;
    document.getElementById(outputId).value = total;
    updateOverallTotalsClose();
}
//Dollars
function calculateTotalDollarsClose(inputId, denominationValue, outputId) {
    const count = parseInt(document.getElementById(inputId).value) || 0;
    const total = count * denominationValue;
    document.getElementById(outputId).value = total;
    updateOverallTotalsCloseDollars();
}
//Yens/JPY
function calculateTotalYensClose(inputId, denominationValue, outputId) {
    const count = parseInt(document.getElementById(inputId).value) || 0;
    const total = count * denominationValue;
    document.getElementById(outputId).value = total;
    updateOverallTotalsCloseYens();
}
// Update overall totals
var closeCurrencyDetalInVnd = [];
function updateOverallTotalsClose() {
    const inputIds = ['oneThsndVndClose', 'twoThsndVndClose', 'fiveThsndVndClose', 'tenThsndVndClose', 'twentyThsndVndClose', 'fiftyThsndVndClose', 'oneLacVndClose', 'twoLacVndClose', 'fiveLacVndClose'];
    const outputIds = ['totalOneThsndVndClose', 'totalTwoThsndVndClose', 'totalFiveThsndVndClose', 'totalTenThsndVndClose', 'totalTwentyThsndVndClose', 'totalFiftyThsndVndClose', 'totalOneLacVndClose', 'totalTwoLacVndClose', 'totalFiveLacVndClose'];

    let totalNotes = 0;
    let totalValue = 0;
    closeCurrencyDetalInVnd = [];

    inputIds.forEach((id, index) => {
        const count = parseInt(document.getElementById(id).value) || 0;
        totalNotes += count;
        const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
        totalValue += value;

        if (count > 0) {
            closeCurrencyDetalInVnd.push({
                denomination: id,
                count: count,
                totalValue: value
            });
        }
    });

    document.getElementById('totalVndClose').value = totalNotes;
    document.getElementById('totalVndCountClose').value = totalValue;
}
// Update overall totals Dollars
var closeCurrencyDetalInDollars = [];
function updateOverallTotalsCloseDollars() {
    const inputIds = ['oneDollarClose', 'fiveDollarsClose', 'tenDollarsClose', 'twentyDollarsClose', 'fiftyDollarsClose', 'hundredDollarsClose'];
    const outputIds = ['totalOneDollarClose', 'totalFiveDollarsClose', 'totalTenDollarsClose', 'totalTwentyDollarsClose', 'totalFiftyDollarsClose', 'totalHundredDollarsClose'];

    let totalNotes = 0;
    let totalValue = 0;
    closeCurrencyDetalInDollars = [];

    inputIds.forEach((id, index) => {
        const count = parseInt(document.getElementById(id).value) || 0;
        totalNotes += count;
        const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
        totalValue += value;

        if (count > 0) {
            closeCurrencyDetalInDollars.push({
                denomination: id,
                count: count,
                totalValue: value
            });
        }
        //Convert the total dollar value to VND
        const usdToVnd = availableCurrenciesCloseStore.find(currency => currency.Name === 'USD');
        //const usdToVnd = 'USD';
        const usdToVndExchangeRate = usdToVnd.ExchangeRate;
        //const usdToVndExchangeRate = 6240;
        const totalValueInVnd = totalValue * usdToVndExchangeRate;
        document.getElementById('totalDollarsClose').value = totalNotes;
        document.getElementById('totalDollarsCountClose').value = totalValue;
        document.getElementById('totalDollarsToVndClose').textContent = totalValueInVnd.toLocaleString('en-US', { minimumFractionDigits: 2 });
    });
}

// Update overall totals Yens / JPY
var closeCurrencyDetalInYens = [];
function updateOverallTotalsCloseYens() {
    const inputIds = ['oneYenClose', 'fiveYensClose', 'tenYensClose', 'fiftyYensClose', 'hundredYensClose', 'fivehundredYensClose', 'onethousandsYensClose', 'twothousandsYensClose', 'fivethousandsYensClose', 'tenthousandsYensClose'];
    const outputIds = ['totalOneYenClose', 'totalFiveYensClose', 'totalTenYensClose', 'totalFiftyYensClose', 'totalHundredYensClose', 'totalFivehundredYensClose', 'totalOnethousandsYensClose', 'totalTwothousandsYensClose', 'totalFivethousandsYensClose', 'totalTenthousandsYensClose'];

    let totalNotes = 0;
    let totalValue = 0;
    closeCurrencyDetalInYens = [];

    inputIds.forEach((id, index) => {
        const count = parseInt(document.getElementById(id).value) || 0;
        totalNotes += count;
        const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
        totalValue += value;

        if (count > 0) {
            closeCurrencyDetalInYens.push({
                denomination: id,
                count: count,
                totalValue: value
            });
        }
        const yenToVnd = availableCurrenciesCloseStore.find(currency => currency.Name === 'JPY');
        //const yenToVnd = 'JPY';
        const yenToVndExchangeRate = yenToVnd.ExchangeRate;
        //const yenToVndExchangeRate = 179;
        const totalValueInVnd = totalValue * yenToVndExchangeRate;
        document.getElementById('totalYensClose').value = totalNotes;
        document.getElementById('totalYensCountClose').value = totalValue;
        document.getElementById('totalYensToVndClose').textContent = totalValueInVnd.toLocaleString('en-US', { minimumFractionDigits: 2 });
    });


}
function PrintRDLCReport() {
    var reportViewer = document.getElementById('ReportViewer1');
    if (reportViewer) {
        reportViewer.contentWindow.print(); // Triggers print
    }
}

function logout() {
    localStorage.clear();
    window.location.href = '/UserManagement/Logout';
}
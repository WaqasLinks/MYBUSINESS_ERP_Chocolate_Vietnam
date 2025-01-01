
var availableCurrencies = [];
$(document).ready(function () {
    getAllAvailableCurrencies();
});
function getAllAvailableCurrencies() {
    $.ajax({
        url: '/Currencies/GetAllAbvailableCurrencies',
        type: 'GET',
        success: function (response) {
            if (response.Success) {
                availableCurrencies = response.Data;
            } else {
                alert('Failed to set session: ' + response.Message);
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred while setting the session: ' + error);
        }
    });
}

//function showModalAndFocusInput() {
//    $('#storeOpeningPopup').modal('show');
//    $('#storeOpeningPopup').on('shown.bs.modal', function () {
//        var initialInput = document.getElementById('oneThsndVnd');
//        if (initialInput) {
//            activeInput = initialInput;
//            initialInput.focus();
//        }
//    });
//}
//// Attach click handler to elements with the 'storeids' class
//$('.storeids').click(function () {
//    // Fetch the currencies first, then show the modal and focus the input
//    getAllAvailableCurrencies();
//});

//Workig Code
$('.storeids').click(function () {
    $('#storeOpeningPopup').modal('show');
    // Set the initial active input field and focus on it
    $('#storeOpeningPopup').on('shown.bs.modal', function () {
        var initialInput = document.getElementById('oneThsndVnd');
        if (initialInput) {
            activeInput = initialInput;
            initialInput.focus();
        }
    });
});

////////////////
var formattedString = "";
var formattedStringDollars = "";
var formattedStringYens = "";
var storeName = "";
let activeInput = null;
document.querySelectorAll('.storeids').forEach(function (element) {
    debugger;
    element.addEventListener('click', function () {
        debugger;
        // Get the value of data-storeid from the clicked element
        const storeIds = this.getAttribute('data-storeid');
        console.log(storeIds);
        //storeName = this.getAttribute('data-storename');
        // Set this storeId on the openShop button's data attribute for later use
        $('#openShop').data('storeid', storeIds);
    });
});
//Bind input values from Calculator after selecting
document.querySelectorAll('input').forEach(input => {
    input.addEventListener('focus', function () {
        activeInput = this;
    });
});

///
//Calculator
// Function to handle calculator button clicks
// Function to handle calculator button clicks
function insertNumber(number) {
    if (activeInput) {
        // If 'CE' is pressed, clear the input field
        if (number === 'CE') {
            activeInput.value = '0';
        }
        // If 'X' is pressed, remove the last character
        else if (number === 'X') {
            activeInput.value = activeInput.value.length === 1 ? '0' : activeInput.value.slice(0, -1);
        }
        // If the current value is '0', replace it with the new number
        else {
            if (activeInput.value === '0') {
                activeInput.value = number;
            } else {
                activeInput.value += number;
            }
        }
        updateTotalForActiveInput();
        updateTotalForActiveInputsDollars();
        updateTotalForActiveInputsYens();
    }
}
///Update totol for active inputs from calculator vnd
function updateTotalForActiveInput() {
    // Map each input field ID to its denomination value and corresponding output field
    const denominationMap = {
        'oneThsndVnd': { denominationValue: 1000, outputId: 'totalOneThsndVnd' },
        'twoThsndVnd': { denominationValue: 2000, outputId: 'totalTwoThsndVnd' },
        'fiveThsndVnd': { denominationValue: 5000, outputId: 'totalFiveThsndVnd' },
        'tenThsndVnd': { denominationValue: 10000, outputId: 'totalTenThsndVnd' },
        'twentyThsndVnd': { denominationValue: 20000, outputId: 'totalTwentyThsndVnd' },
        'fiftyThsndVnd': { denominationValue: 50000, outputId: 'totalFiftyThsndVnd' },
        'oneLacVnd': { denominationValue: 100000, outputId: 'totalOneLacVnd' },
        'twoLacVnd': { denominationValue: 200000, outputId: 'totalTwoLacVnd' },
        'fiveLacVnd': { denominationValue: 500000, outputId: 'totalFiveLacVnd' }
    };

    const inputId = activeInput.id; // Get the ID of the active input field
    const denomination = denominationMap[inputId]; // Get the denomination details for the active input field

    if (denomination) {
        // Call calculateTotal with the current inputId, denominationValue, and corresponding outputId
        calculateTotal(inputId, denomination.denominationValue, denomination.outputId);
    }
}
//Update Total dollars from keyboard
function updateTotalForActiveInputsDollars() {
    // Map each input field ID to its denomination value and corresponding output field
    const denominationMap = {
        'oneDollar': { denominationValue: 1, outputId: 'totalOneDollar' },
        'fiveDollars': { denominationValue: 5, outputId: 'totalFiveDollars' },
        'tenDollars': { denominationValue: 10, outputId: 'totalTenDollars' },
        'twentyDollars': { denominationValue: 20, outputId: 'totalTwentyDollars' },
        'fiftyDollars': { denominationValue: 50, outputId: 'totalFiftyDollars' },
        'hundredDollars': { denominationValue: 100, outputId: 'totalHundredDollars' }
    };

    const inputId = activeInput.id; // Get the ID of the active input field
    const denomination = denominationMap[inputId]; // Get the denomination details for the active input field

    if (denomination) {
        // Call calculateTotal with the current inputId, denominationValue, and corresponding outputId
        calculateTotalDollars(inputId, denomination.denominationValue, denomination.outputId);
    }
}
//
//Update Total Yens from keyboard
function updateTotalForActiveInputsYens() {
    // Map each input field ID to its denomination value and corresponding output field
    const denominationMap = {
        'oneYen': { denominationValue: 1, outputId: 'totalOneYen' },
        'fiveYens': { denominationValue: 5, outputId: 'totalFiveYens' },
        'tenYens': { denominationValue: 10, outputId: 'totalTenYens' },
        'fiftyYens': { denominationValue: 50, outputId: 'totalFiftyYens' },
        'hundredYens': { denominationValue: 100, outputId: 'totalHundredYens' },
        'fivehundredYens': { denominationValue: 500, outputId: 'totalFivehundredYens' },
        'onethousandsYens': { denominationValue: 1000, outputId: 'totalOnethousandsYens' },
        'twothousandsYens': { denominationValue: 2000, outputId: 'totalTwothousandsYens' },
        'fivethousandsYens': { denominationValue: 5000, outputId: 'totalFivethousandsYens' },
        'tenthousandsYens': { denominationValue: 10000, outputId: 'totalTenthousandsYens' },
    };

    const inputId = activeInput.id; // Get the ID of the active input field
    const denomination = denominationMap[inputId]; // Get the denomination details for the active input field

    if (denomination) {
        // Call calculateTotal with the current inputId, denominationValue, and corresponding outputId
        calculateTotalYens(inputId, denomination.denominationValue, denomination.outputId);
    }
}
//
//Label management
function labelClickVND(event) {
    var label = event.target;
    var value = label.getAttribute('data-value'); // Get the value associated with the label

    if (activeInput && value) {
        activeInput.value = value;
        updateTotalForActiveInput();
    }
}
// Add event listeners to all labels to handle clicks
document.querySelectorAll('label[id^="lbl"]').forEach(label => {
    label.addEventListener('click', labelClickVND);
});
//Dollars
function labelClickDollars(event) {
    var label = event.target;
    var value = label.getAttribute('data-value-dollars'); // Get the value associated with the label

    if (activeInput && value) {
        activeInput.value = value;
        updateTotalForActiveInputsDollars();
    }
}
// Add event listeners to all labels to handle clicks
document.querySelectorAll('label[id^="lbl"]').forEach(label => {
    label.addEventListener('click', labelClickDollars);
});
//YEN
function labelClickYens(event) {
    var label = event.target;
    var value = label.getAttribute('data-value-yens'); // Get the value associated with the label

    if (activeInput && value) {
        activeInput.value = value;
        updateTotalForActiveInputsYens();
    }
}
// Add event listeners to all labels to handle clicks
document.querySelectorAll('label[id^="lbl"]').forEach(label => {
    label.addEventListener('click', labelClickYens);
});
//
$('#openShop').click(function () {
    const storeIds = $(this).data('storeid');
    if (!storeIds) {
        alert('No store selected. Please select a store first.');
        return;
    }
    var totalAmountVnd = document.getElementById('totalVndCount').value;
    if (totalAmountVnd == '0') totalAmountVnd = 0;
    if (confirm(`You input '${totalAmountVnd}', are you sure?`)) {
        // Select the element
        //const element = document.querySelector('.storeids');
        //// Get the value of data-storeid
        //const storeIds = element.getAttribute('data-storeid');

        if (openingCurrencyDetalInVnd.length > 0) {
            formattedString = openingCurrencyDetalInVnd.map(item => {
                // Set the fixed denomination value
                let denominationValue = 0;

                switch (item.denomination) {
                    case 'oneThsndVnd':
                        denominationValue = 1000;
                        break;
                    case 'twoThsndVnd':
                        denominationValue = 2000;
                        break;
                    case 'fiveThsndVnd':
                        denominationValue = 5000;
                        break;
                    case 'tenThsndVnd':
                        denominationValue = 10000;
                        break;
                    case 'twentyThsndVnd':
                        denominationValue = 20000;
                        break;
                    case 'fiftyThsndVnd':
                        denominationValue = 50000;
                        break;
                    case 'oneLacVnd':
                        denominationValue = 100000;
                        break;
                    case 'twoLacVnd':
                        denominationValue = 200000;
                        break;
                    case 'fiveLacVnd':
                        denominationValue = 500000;
                        break;
                    default:
                        denominationValue = 0;
                }

                // Use the fixed denomination value in the formatted string
                return `${denominationValue}@${item.count}`;
            }).join(':');
        }
        if (openingCurrencyDetalInDollar.length > 0) {
            formattedStringDollars = openingCurrencyDetalInDollar.map(item => {
                // Set the fixed denomination value
                let denominationValue = 0;

                switch (item.denomination) {
                    case 'oneDollar':
                        denominationValue = 1;
                        break;
                    case 'fiveDollars':
                        denominationValue = 5;
                        break;
                    case 'tenDollars':
                        denominationValue = 10;
                        break;
                    case 'twentyDollars':
                        denominationValue = 20;
                        break;
                    case 'fiftyDollars':
                        denominationValue = 50;
                        break;
                    default:
                        denominationValue = 0;
                }

                // Use the fixed denomination value in the formatted string
                return `${denominationValue}@${item.count}`;
            }).join(':');
        }
        if (openingCurrencyDetalInYens.length > 0) {
            formattedStringYens = openingCurrencyDetalInYens.map(item => {
                // Set the fixed denomination value
                let denominationValue = 0;

                switch (item.denomination) {
                    case 'oneYen':
                        denominationValue = 1;
                        break;
                    case 'fiveYens':
                        denominationValue = 5;
                        break;
                    case 'tenYens':
                        denominationValue = 10;
                        break;
                    case 'fiftyYens':
                        denominationValue = 50;
                        break;
                    case 'hundredYens':
                        denominationValue = 100;
                        break;
                    case 'fivehundredYens':
                        denominationValue = 100;
                        break;
                    case 'onethousandsYens':
                        denominationValue = 1000;
                        break;
                    case 'twothousandsYens':
                        denominationValue = 2000;
                        break;
                    case 'fivethousandsYens':
                        denominationValue = 5000;
                        break;
                    case 'tenthousandsYens':
                        denominationValue = 10000;
                        break;
                    default:
                        denominationValue = 0;
                }

                // Use the fixed denomination value in the formatted string
                return `${denominationValue}@${item.count}`;
            }).join(':');
        }
        // Set storeId in localStorage and session
        //storeId = storeIds;
        if (storeIds != null || storeIds != undefined) {
            //alert("Open Shop" + storeIds)
            localStorage.setItem('storeIds', storeIds);
            openShop();
            //localStorage.setItem('storeName', storeName);

            //Working old
            //setSession(); // Call setSession function to make an AJAX request
        }
    } else {
        alert("Operation cancelled.");
        $('#storeOpeningPopup').modal('hide');
        $('#storeOpeningPopup').find('input, textarea, select').val('0');
    }
});

//$('#openShop').click(function () {
//    if (confirm("You input 0, are you sure?")) {
//        debugger;
//        // Select the element
//        const element = document.querySelector('.storeids');
//        // Get the value of data-storeid
//        const storeIds = element.getAttribute('data-storeid');
//        var formattedString = "";
//        if (openingCurrencyDetalInVnd.length > 0) {
//            debugger;
//            formattedString = openingCurrencyDetalInVnd.map(item => {
//                // Set the fixed denomination value
//                let denominationValue = 0;

//                switch (item.denomination) {
//                    case 'oneThsndVnd':
//                        denominationValue = 1000;
//                        break;
//                    case 'twoThsndVnd':
//                        denominationValue = 2000;
//                        break;
//                    case 'fiveThsndVnd':
//                        denominationValue = 5000;
//                        break;
//                    case 'tenThsndVnd':
//                        denominationValue = 10000;
//                        break;
//                    case 'twentyThsndVnd':
//                        denominationValue = 20000;
//                        break;
//                    case 'fiftyThsndVnd':
//                        denominationValue = 50000;
//                        break;
//                    case 'oneLacVnd':
//                        denominationValue = 100000;
//                        break;
//                    case 'twoLacVnd':
//                        denominationValue = 200000;
//                        break;
//                    case 'fiveLacVnd':
//                        denominationValue = 500000;
//                        break;
//                    default:
//                        denominationValue = 0;
//                }

//                // Use the fixed denomination value in the formatted string
//                return `${denominationValue}@${item.count}`;
//            }).join(':');
//        }
//        //formattedString = openingCurrencyDetalInVnd.map(item => `${item.totalValue}@${item.count}`).join(':');
//        //storeId from layout/shared.js
//        storeId = storeIds;
//        if (storeIds != null || storeIds != undefined) { localStorage.setItem('storeId', storeId); /*document.getElementById('closeStore').style.display = 'block';*/ /*document.getElementById("StorageItem").value = storeId;*/ setSession(); }

//        var selectedBlance;
//        var vndBalance = parseFloat($('#totalVndCount').val());
//        var dollarBalance = parseFloat($('#totalDollarsCount').val());
//        var jpyBalance = parseFloat($('#totalYensCount').val());

//        if (!isNaN(vndBalance) && vndBalance !== 0) {
//            selectedBlance = vndBalance;
//        } else if (!isNaN(dollarBalance) && dollarBalance !== 0) {
//            selectedBlance = dollarBalance;
//        } else if (!isNaN(jpyBalance) && jpyBalance !== 0) {
//            selectedBlance = jpyBalance;
//        } else {
//            alert('No balance available');
//        }

//        var storeViewModel = {
//            OpeningBalance: selectedBlance, // Assuming you have an input field with id="openingBalance"
//            OpeningCurrencyDetail: formattedString
//        };

//        // Make the AJAX POST request to server-side
//        $.ajax({
//            url: '/Stores/OpenShop',  // Replace 'ControllerName' with your actual controller name
//            type: 'POST',
//            data: JSON.stringify(storeViewModel),
//            contentType: 'application/json; charset=utf-8',
//            dataType: 'json',
//            //headers: {
//            //    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // For anti-forgery token
//            //},
//            success: function (response) {
//                if (response.Success) {
//                    //alert(response.Message);
//                    window.location.href = '/SOSR/Index';
//                    $('#storeOpeningPopup').modal('hide');
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
//    }
//});

//Working Code

//VND
function calculateTotal(inputId, denominationValue, outputId) {
    const count = parseInt(document.getElementById(inputId).value) || 0;
    const total = count * denominationValue;
    document.getElementById(outputId).value = total;

    // Update the overall totals
    updateOverallTotals();
}
//Dollars
function calculateTotalDollars(inputId, denominationValue, outputId) {
    const count = parseInt(document.getElementById(inputId).value) || 0;
    const total = count * denominationValue;
    document.getElementById(outputId).value = total;

    // Update the overall totals dollars
    updateOverallDollarsTotal();
}
//Yens
function calculateTotalYens(inputId, denominationValue, outputId) {
    const count = parseInt(document.getElementById(inputId).value) || 0;
    const total = count * denominationValue;
    document.getElementById(outputId).value = total;

    // Update the overall totals dollars
    updateOverallYensTotal();
}
//Vnd Total
var openingCurrencyDetalInVnd = [];
function updateOverallTotals() {
    const inputIds = ['oneThsndVnd', 'twoThsndVnd', 'fiveThsndVnd', 'tenThsndVnd', 'twentyThsndVnd', 'fiftyThsndVnd', 'oneLacVnd', 'twoLacVnd', 'fiveLacVnd'];
    const outputIds = ['totalOneThsndVnd', 'totalTwoThsndVnd', 'totalFiveThsndVnd', 'totalTenThsndVnd', 'totalTwentyThsndVnd', 'totalFiftyThsndVnd', 'totalOneLacVnd', 'totalTwoLacVnd', 'totalFiveLacVnd'];

    let totalNotes = 0;
    let totalValue = 0;
    openingCurrencyDetalInVnd = [];
    inputIds.forEach((id, index) => {
        const count = parseInt(document.getElementById(id).value) || 0;
        totalNotes += count;

        // Get the corresponding total value
        const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
        totalValue += value;

        // Push the note count and value to the array
        if (count > 0) {
            openingCurrencyDetalInVnd.push({
                denomination: id,  // Identifier for the denomination (e.g., 'oneThsndVnd')
                count: count,
                totalValue: value
            });
        }
    });
    document.getElementById('totalVnd').value = totalNotes;
    document.getElementById('totalVndCount').value = totalValue;
}
//Dollars Total
var openingCurrencyDetalInDollar = [];
function updateOverallDollarsTotal() {
    const inputIds = ['oneDollar', 'fiveDollars', 'tenDollars', 'twentyDollars', 'fiftyDollars', 'hundredDollars'];
    const outputIds = ['totalOneDollar', 'totalFiveDollars', 'totalTenDollars', 'totalTwentyDollars', 'totalFiftyDollars', 'totalHundredDollars'];
    let totalNotes = 0;
    let totalValue = 0;
    openingCurrencyDetalInDollar = [];
    const usdToVnd = availableCurrencies.find(currency => currency.Name === 'USD');
    debugger;
    if (usdToVnd) {
        const usdToVndExchangeRate = usdToVnd.ExchangeRate;  // Get the exchange rate for USD to VND

        // Now use the exchange rate to calculate total notes and dollar value
        inputIds.forEach((id, index) => {
            const count = parseInt(document.getElementById(id).value) || 0;
            totalNotes += count;

            // Get the corresponding total value for each denomination
            const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
            totalValue += value;

            // Push the note count and value to the array
            if (count > 0) {
                openingCurrencyDetalInDollar.push({
                    denomination: id,  // Identifier for the denomination (e.g., 'oneDollar')
                    count: count,
                    totalValue: value
                });
            }
            //Convert the total dollar value to VND
            const totalValueInVnd = totalValue * usdToVndExchangeRate;

            // Update the DOM elements with the totals
            document.getElementById('totalDollars').value = totalNotes;
            document.getElementById('totalDollarsCount').value = totalValue;

            // Set the converted total value in VND with two decimal places
            document.getElementById('totalDollarsToVnd').textContent = totalValueInVnd.toLocaleString('en-US', { minimumFractionDigits: 2 });
        });

        console.log('USD to VND Exchange Rate:', usdToVndExchangeRate);
        // You can perform additional operations with the exchange rate here
    } else {
        alert('USD currency data not found.');
    }
    // Fetch the exchange rate using .then() instead of async/await
    //const exchangeRateApiUrl = 'https://v6.exchangerate-api.com/v6/93f26f8cc2c57462af9ec3be/latest/USD';
    //fetch(exchangeRateApiUrl)
    //    .then(response => response.json())  // Convert the response to JSON
    //    .then(data => {
    //        // Get the USD to VND exchange rate
    //        const usdToVndExchangeRate = data.conversion_rates.VND;

    //        // Calculate total notes and dollar value
    //        inputIds.forEach((id, index) => {
    //            const count = parseInt(document.getElementById(id).value) || 0;
    //            totalNotes += count;

    //            // Get the corresponding total value for each denomination
    //            const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
    //            totalValue += value;

    //            // Push the note count and value to the array
    //            if (count > 0) {
    //                openingCurrencyDetalInDollar.push({
    //                    denomination: id,  // Identifier for the denomination (e.g., 'oneDollar')
    //                    count: count,
    //                    totalValue: value
    //                });
    //            }
    //        });

    //        // Convert the total dollar value to VND
    //        const totalValueInVnd = totalValue * usdToVndExchangeRate;

    //        // Update the DOM elements with the totals
    //        document.getElementById('totalDollars').value = totalNotes;
    //        document.getElementById('totalDollarsCount').value = totalValue;

    //        // Set the converted total value in VND with two decimal places
    //        document.getElementById('totalDollarsToVnd').textContent = totalValueInVnd.toLocaleString('en-US', { minimumFractionDigits: 2 });

    //    })
    //    .catch(error => {
    //        console.error("Error fetching exchange rate: ", error);
    //        document.getElementById('totalDollarsToVnd').textContent = "Error fetching exchange rate";
    //    });

    //inputIds.forEach((id, index) => {
    //    const count = parseInt(document.getElementById(id).value) || 0;
    //    totalNotes += count;
    //    // Get the corresponding total value
    //    const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
    //    totalValue += value;
    //    document.getElementById('totalDollarsToVnd').textContent = (totalValue).toFixed(2);
    //    // Push the note count and value to the array
    //    if (count > 0) {
    //        openingCurrencyDetalInDollar.push({
    //            denomination: id,  // Identifier for the denomination (e.g., 'oneThsndVnd')
    //            count: count,
    //            totalValue: value
    //        });
    //    }
    //});

    //document.getElementById('totalDollars').value = totalNotes;
    //document.getElementById('totalDollarsCount').value = totalValue;
}
//Yen Total
var openingCurrencyDetalInYens = [];
function updateOverallYensTotal() {
    const inputIds = ['oneYen', 'fiveYens', 'tenYens', 'fiftyYens', 'hundredYens', 'fivehundredYens', 'onethousandsYens', 'twothousandsYens', 'fivethousandsYens', 'tenthousandsYens'];
    const outputIds = ['totalOneYen', 'totalFiveYens', 'totalTenYens', 'totalFiftyYens', 'totalHundredYens', 'totalFivehundredYens', 'totalOnethousandsYens', 'totalTwothousandsYens', 'totalFivethousandsYens', 'totalTenthousandsYens'];
    let totalNotes = 0;
    let totalValue = 0;
    openingCurrencyDetalInYens = [];
    const yensToVnd = availableCurrencies.find(currency => currency.Name === 'JPY');
    //const yensToVnd = 'JPY';
    if (yensToVnd) {
        const yenToVndExchangeRate = yensToVnd.ExchangeRate;  // Get the exchange rate for USD to VND
        //const yenToVndExchangeRate = 179;  // Get the exchange rate for USD to VND

        // Now use the exchange rate to calculate total notes and dollar value
        inputIds.forEach((id, index) => {
            const count = parseInt(document.getElementById(id).value) || 0;
            totalNotes += count;

            // Get the corresponding total value for each denomination
            const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
            totalValue += value;

            // Push the note count and value to the array
            if (count > 0) {
                openingCurrencyDetalInDollar.push({
                    denomination: id,  // Identifier for the denomination (e.g., 'oneDollar')
                    count: count,
                    totalValue: value
                });
            }
            // Convert the total dollar value to VND
            const totalValueInVnd = totalValue * yenToVndExchangeRate;

            // Update the DOM elements with the totals
            document.getElementById('totalYens').value = totalNotes;
            document.getElementById('totalYensCount').value = totalValue;

            // Set the converted total value in VND with two decimal places
            document.getElementById('totalYensToVnd').textContent = totalValueInVnd.toLocaleString('en-US', { minimumFractionDigits: 2 });
        });

        console.log('JPY to VND Exchange Rate:', usdToVndExchangeRate);
        // You can perform additional operations with the exchange rate here
    } else {
        alert('JPY currency data not found.');
    }


    //const exchangeRateApiUrl = 'https://v6.exchangerate-api.com/v6/93f26f8cc2c57462af9ec3be/latest/JPY';
    //// Fetch the exchange rate using .then() instead of async/await
    //fetch(exchangeRateApiUrl)
    //    .then(response => response.json())  // Convert the response to JSON
    //    .then(data => {
    //        // Get the USD to VND exchange rate
    //        const yenToVndExchangeRate = data.conversion_rates.VND;

    //        // Calculate total notes and dollar value
    //        inputIds.forEach((id, index) => {
    //            const count = parseInt(document.getElementById(id).value) || 0;
    //            totalNotes += count;

    //            // Get the corresponding total value for each denomination
    //            const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
    //            totalValue += value;

    //            // Push the note count and value to the array
    //            if (count > 0) {
    //                openingCurrencyDetalInYens.push({
    //                    denomination: id,  // Identifier for the denomination (e.g., 'oneDollar')
    //                    count: count,
    //                    totalValue: value
    //                });
    //            }
    //        });

    //        // Convert the total dollar value to VND
    //        const totalValueInVnd = totalValue * yenToVndExchangeRate;

    //        // Update the DOM elements with the totals
    //        document.getElementById('totalYens').value = totalNotes;
    //        document.getElementById('totalYensCount').value = totalValue;

    //        // Set the converted total value in VND with two decimal places
    //        document.getElementById('totalYensToVnd').textContent = totalValueInVnd.toLocaleString('en-US', { minimumFractionDigits: 2 });

    //    })
    //    .catch(error => {
    //        console.error("Error fetching exchange rate: ", error);
    //        document.getElementById('totalYensToVnd').textContent = "Error fetching exchange rate";
    //    });



    //inputIds.forEach((id, index) => {
    //    const count = parseInt(document.getElementById(id).value) || 0;
    //    totalNotes += count;

    //    // Get the corresponding total value
    //    const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
    //    totalValue += value;
    //    document.getElementById('totalYensToVnd').textContent = (totalValue).toFixed(2);
    //    // Push the note count and value to the array
    //    if (count > 0) {
    //        openingCurrencyDetalInYens.push({
    //            denomination: id,  // Identifier for the denomination (e.g., 'oneThsndVnd')
    //            count: count,
    //            totalValue: value
    //        });
    //    }
    //});
    //document.getElementById('totalYens').value = totalNotes;
    //document.getElementById('totalYensCount').value = totalValue;
}

//function updateOverallDollarsTotal() {
//    debugger;
//    fetchDollarValue()
//        .then(vndValue => {
//            let totalNotes = 0;
//            let totalValue = 0;
//            const inputIds = ['oneDollar', 'fiveDollars', 'tenDollars', 'twentyDollars', 'fiftyDollars', 'hundredDollars'];
//            const outputIds = ['totalOneDollar', 'totalFiveDollars', 'totalTenDollars', 'totalTwentyDollars', 'totalFiftyDollars', 'totalHundredDollars'];
//            debugger;
//            // Array to store details
//            openingCurrencyDetalInDollar = [];

//            inputIds.forEach((id, index) => {
//                const count = parseInt(document.getElementById(id).value) || 0;
//                totalNotes += count;

//                // Get the corresponding total value
//                const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
//                totalValue += value;

//                // Update total value in VND
//                document.getElementById('totalDollarsToVnd').textContent = (totalValue * vndValue).toFixed(2); // Ensure correct formatting

//                // Push details to the array if count is greater than 0
//                if (count > 0) {
//                    openingCurrencyDetalInDollar.push({
//                        denomination: id,
//                        count: count,
//                        totalValue: value
//                    });
//                }
//            });
//            debugger;
//            // Update overall totals for notes and values
//            document.getElementById('totalDollars').value = totalNotes;
//            document.getElementById('totalDollarsCount').value = totalValue;
//        })
//        .catch(error => {
//            console.error('Failed to fetch VND value:', error);
//        });

//    //fetchDollarValue()
//    //    .then(vndValue => {
//    //        let totalNotes = 0;
//    //        let totalValue = 0;
//    //        const inputIds = ['oneDollar', 'fiveDollars', 'tenDollars', 'twentyDollars', 'fiftyDollars', 'hundredDollars'];
//    //        const outputIds = ['totalOneDollar', 'totalFiveDollars', 'totalTenDollars', 'totalTwentyDollars', 'totalFiftyDollars', 'totalHundredDollars'];
//    //        debugger;
//    //        // Array to store details
//    //        openingCurrencyDetalInDollar = [];

//    //        inputIds.forEach((id, index) => {
//    //            const count = parseInt(document.getElementById(id).value) || 0;
//    //            totalNotes += count;

//    //            // Get the corresponding total value
//    //            const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
//    //            totalValue += value;

//    //            // Update total value in VND
//    //            document.getElementById('totalDollarsToVnd').textContent = (totalValue * vndValue).toFixed(2); // Ensure correct formatting

//    //            // Push details to the array if count is greater than 0
//    //            if (count > 0) {
//    //                openingCurrencyDetalInDollar.push({
//    //                    denomination: id,
//    //                    count: count,
//    //                    totalValue: value
//    //                });
//    //            }
//    //        });
//    //        debugger;
//    //        // Update overall totals for notes and values
//    //        document.getElementById('totalDollars').value = totalNotes;
//    //        document.getElementById('totalDollarsCount').value = totalValue;
//    //    })
//    //    .catch(error => {
//    //        console.error('Failed to fetch VND value:', error);
//    //    });
//}

//function updateOverallYensTotal() {
//    debugger;
//    fetchYensValue()
//        .then(vndValue => {
//            let totalNotes = 0;
//            let totalValue = 0;
//            const inputIds = ['oneYen', 'fiveYens', 'tenYens', 'fiftyYens', 'hundredYens', 'fivehundredYens', 'onethousandsYens', 'twothousandsYens', 'fivethousandsYens', 'tenthousandsYens'];
//            const outputIds = ['totalOneYen','totalFiveYens', 'totalTenYens', 'totalFiftyYens', 'totalHundredYens', 'totalFivehundredYens', 'totalOnethousandsYens', 'totalTwothousandsYens', 'totalFivethousandsYens', 'totalTenthousandsYens'];

//            // Array to store details
//            openingCurrencyDetalInYens = [];

//            inputIds.forEach((id, index) => {
//                const count = parseInt(document.getElementById(id).value) || 0;
//                totalNotes += count;

//                // Get the corresponding total value
//                const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
//                totalValue += value;

//                // Update total value in VND
//                document.getElementById('totalYensToVnd').textContent = (totalValue * vndValue).toFixed(2); // Ensure correct formatting

//                // Push details to the array if count is greater than 0
//                if (count > 0) {
//                    openingCurrencyDetalInYens.push({
//                        denomination: id,
//                        count: count,
//                        totalValue: value
//                    });
//                }
//            });

//            // Update overall totals for notes and values
//            document.getElementById('totalYens').value = totalNotes;
//            document.getElementById('totalYensCount').value = totalValue;
//        })
//        .catch(error => {
//            console.error('Failed to fetch Yens value:', error);
//        });
//}

//function updateOverallDollarsTotal() {
//    debugger;
//    var currentVndValue = null;
//    //fetchDollarValue()
//    //    .then(vndValue => {
//    //        currentVndValue = vndValue;
//    //    });
//    const inputIds = ['oneDollar', 'fiveDollars', 'tenDollars', 'twentyDollars', 'fiftyDollars', 'hundredDollars'];
//    const outputIds = ['totalOneDollar', 'totalFiveDollars', 'totalTenDollars', 'totalTwentyDollars', 'totalFiftyDollars', 'totalHundredDollars'];

//    let totalNotes = 0;
//    let totalValue = 0;
//    openingCurrencyDetalInDollar = [];
//    inputIds.forEach((id, index) => {
//        const count = parseInt(document.getElementById(id).value) || 0;
//        totalNotes += count;
//        debugger
//        // Get the corresponding total value
//        const value = parseInt(document.getElementById(outputIds[index]).value) || 0;
//        totalValue += value;
//        // Update total value in VND
//        //document.getElementById('totalDollarsToVnd').textContent = (totalValue * vndValue).toFixed(2); // Ensure correct formatting
//        debugger;
//        // Push the note count and value to the array
//        if (count > 0) {
//            openingCurrencyDetalInDollar.push({
//                denomination: id,  // Identifier for the denomination (e.g., 'oneThsndVnd')
//                count: count,
//                totalValue: value
//            });
//        }
//    });
//    //inputIds.forEach((id, index) => {
//    //    const count = parseInt(document.getElementById(id).value) || 0;
//    //    totalNotes += count;
//    //});

//    //outputIds.forEach(id => {
//    //    const value = parseInt(document.getElementById(id).value) || 0;
//    //    totalValue += value;
//    //});

//    document.getElementById('totalDollars').value = totalNotes;
//    document.getElementById('totalDollarsCount').value = totalValue;
//}

//Fetch real tim dollar value
//function fetchDollarValue() {
//    debugger;
//    const endpoint = 'latest';
//    const access_key = '2094584b8c30d29767727fe523d77054'; // Ensure this key is valid

//    return new Promise((resolve, reject) => {
//        $.ajax({
//            url: `http://data.fixer.io/api/${endpoint}?access_key=${access_key}`,
//            dataType: 'jsonp',
//            success: function (json) {
//                debugger;
//                if (json && json.rates) {
//                    console.log('API response:', json); // Log the whole response for debugging

//                    // Fixer.io defaults to EUR base currency in free tier
//                    const usdToEur = json.rates.USD;
//                    const vndToEur = json.rates.VND;

//                    if (usdToEur && vndToEur) {
//                        const usdToVnd = vndToEur / usdToEur; // Calculate USD to VND conversion
//                        console.log(`USD to VND exchange rate: ${usdToVnd}`);
//                        resolve(usdToVnd);
//                    } else {
//                        reject('USD or VND rate not found in the response');
//                    }
//                } else {
//                    reject('Invalid API response, rates not found');
//                }
//            },
//            error: function (xhr, status, error) {
//                console.error('Error fetching data:', error);
//                reject('Error fetching data: ' + error);
//            }
//        });
//    });
//}

//function fetchYensValue() {
//    const endpoint = 'latest';
//    const access_key = 'f04adc884f235403e0b71ded198c9f8a';

//    return new Promise((resolve, reject) => {
//        $.ajax({
//            url: `http://data.fixer.io/api/${endpoint}?access_key=${access_key}`,
//            dataType: 'jsonp',
//            success: function (json) {
//                if (json && json.rates) {
//                    resolve(json.rates.VND); // Resolve the promise with the dollar value
//                } else {
//                    reject('Dollar value not found');
//                }
//            },
//            error: function (xhr, status, error) {
//                reject('Error fetching data: ' + error);
//            }
//        });
//    });
//}
//function fetchDollarValue() {
//    debugger;
//    endpoint = 'latest'
//    access_key = 'f04adc884f235403e0b71ded198c9f8a';

//    // get the most recent exchange rates via the "latest" endpoint:
//    $.ajax({
//        url: 'http://data.fixer.io/api/' + endpoint + '?access_key=' + access_key,
//        dataType: 'jsonp',
//        success: function (json) {
//            debugger;
//            // exchange rata data is stored in json.rates
//            alert(json.rates.GBP);

//            // base currency is stored in json.base
//            alert(json.base);

//            // timestamp can be accessed in json.timestamp
//            alert(json.timestamp);
//            return json.USD;
//        }
//    });
//    //const apiKey = 'f04adc884f235403e0b71ded198c9f8a';
//    //return fetch(`https://api.yourchosenapi.com/latest?access_key=${apiKey}`)
//    //    .then(response => {
//    //        if (!response.ok) {
//    //            throw new Error('Network response was not ok');
//    //        }
//    //        return response.json();
//    //    })
//    //    .then(data => data.rates.USD) // Modify based on actual API response structure
//    //    .catch(error => {
//    //        console.error('There was a problem with the fetch operation:', error);
//    //        return null;
//    //    });
//}
//



//function setSession() {
//    debugger;
//        // Retrieve the value from localStorage
//        var storeIds = localStorage.getItem('storeId');
//        if (storeIds !== null && storeIds !== undefined) {
//            // Set display style and value
//            //document.getElementById('closeStore').style.display = 'block';
//            document.getElementById('storeIdInput').value = storeId;
//            // Submit the form
//            document.getElementById('hiddenForm').submit();
//        }
//}
//function setSession() {
//    var storeIds = localStorage.getItem('storeId');
//    var storesName = localStorage.getItem('storeName');
//    alert(storeIds);
//    if (storeIds !== null && storeIds !== undefined && storeIds!=="") {
//        // Perform an AJAX call to set the session value on the server
//        $.ajax({
//            url: '/UserManagement/StoreValue',  // Replace with your actual controller/action path
//            type: 'POST',
//            //type: 'GET',
//            data: { storeId: storeIds, storeName: storesName },
//            success: function (response) {
//                if (response.Success) {
//                    // Proceed to open the shop after successfully setting the session
//                    openShop();
//                } else {
//                    alert('Failed to set session: ' + response.Message);
//                }
//            },
//            error: function (xhr, status, error) {
//                alert('An error occurred while setting the session: ' + error);
//            }
//        });
//    }
//}
//function openShop() {
//    //var selectedBlance = 0;
//    //var selectedBlanceDollars = 0;
//    //var selectedBlanceYens = 0;
//    //var vndBalance = parseFloat($('#totalVndCount').val());
//    //var dollarBalance = parseFloat($('#totalDollarsCount').val());
//    //var jpyBalance = parseFloat($('#totalYensCount').val());
//    //debugger;
//    //if (!isNaN(vndBalance) && vndBalance !== 0) {
//    //    selectedBlance = vndBalance;
//    //} else if (!isNaN(dollarBalance) && dollarBalance !== 0) {
//    //    debugger;
//    //    selectedBlanceDollars = dollarBalance;
//    //} else if (!isNaN(jpyBalance) && jpyBalance !== 0) {
//    //    selectedBlanceYens = jpyBalance;
//    //} else {
//    //    vndBalance = 0;
//    //    dollarBalance = 0;
//    //    jpyBalance = 0;
//    //    //alert('No balance available');
//    //    //return;
//    //}

//    //var storeViewModel = {
//    //    OpeningBalance: selectedBlance,
//    //    OpeningBalanceDollars: selectedBlanceDollars,
//    //    OpeningCurrencyDetail: formattedString,
//    //    OpeningCurrencyDetailDollars: formattedStringDollars
//    //};
//    var selectedBlance = 0;        // VND balance
//    var selectedBlanceDollars = 0; // Dollar balance
//    var selectedBlanceYens = 0;    // JPY balance
//    var vndBalance = parseFloat($('#totalVndCount').val()) || 0;
//    var dollarBalance = parseFloat($('#totalDollarsCount').val()) || 0;
//    var jpyBalance = parseFloat($('#totalYensCount').val()) || 0;
//    // Check balances
//    if (!isNaN(vndBalance) && vndBalance !== 0) {
//        selectedBlance = vndBalance;
//    }
//    if (!isNaN(dollarBalance) && dollarBalance !== 0) {
//        selectedBlanceDollars = dollarBalance;
//    }
//    if (!isNaN(jpyBalance) && jpyBalance !== 0) {
//        selectedBlanceYens = jpyBalance;
//    }
//    else {
//        // If none of the balances are valid, set everything to 0
//        vndBalance = 0;
//        dollarBalance = 0;
//        jpyBalance = 0;
//    }

//    // Prepare the ViewModel with proper values
//    var storeViewModel = {
//        OpeningBalance: selectedBlance || 0,                    // Ensure it's not undefined
//        OpeningBalanceDollars: selectedBlanceDollars || 0,      // Ensure it's not undefined
//        OpeningBalanceYens: selectedBlanceYens || 0,      // Ensure it's not undefined
//        OpeningCurrencyDetail: formattedString || "",           // Ensure string is initialized
//        OpeningCurrencyDetailDollars: formattedStringDollars || "", // Ensure string is initialized
//        OpeningCurrencyDetailYens: formattedStringYens || "" // Ensure string is initialized
//    };

//    // Make the AJAX POST request to server-side
//    $.ajax({
//        url: '/Stores/OpenShop',
//        type: 'POST',
//        data: JSON.stringify(storeViewModel),
//        contentType: 'application/json; charset=utf-8',
//        dataType: 'json',
//        success: function (response) {
//            debugger;
//            if (response.Success) {
//                debugger;
//                window.location.href = '/SOSR/Create?IsReturn=false';
//                $('#storeOpeningPopup').modal('hide');
//            } else {
//                alert('Error: ' + response.Message);
//            }
//        },
//        error: function (xhr, status, error) {
//            alert('An error occurred: ' + error);
//        }
//    });
//}

//function setSession() {
//    var storeId = localStorage.getItem('storeIds');
//    //var storeName = localStorage.getItem('storeName');
//    alert("Set Session 1" + storeId);
//    if (storeId !== null && storeId !== undefined && storeId !== "") {
//        alert("Set Session 2" + storeId);
//        $.ajax({
//            url: '/UserManagement/StoreValue',
//            type: 'POST', // Assuming you want to use POST for updating server session
//            //data: { storeId: storeId, storeName: storeName },
//            data: { storeId: storeId },
//            success: function (response) {
//                if (response.Success) {
//                    openShop();
//                } else {
//                    alert('Failed to set session: ' + response.Message);
//                }
//            },
//            error: function (xhr, status, error) {
//                alert('An error occurred while setting the session: ' + error);
//            }
//        });
//    }
//}

// Function to open the shop after setting session
function openShop() {
    var vndBalance = parseFloat($('#totalVndCount').val()) || 0;
    var dollarBalance = parseFloat($('#totalDollarsCount').val()) || 0;
    var jpyBalance = parseFloat($('#totalYensCount').val()) || 0;
    var _storeId = localStorage.getItem('storeIds');
    var storeId = parseInt(_storeId, 10);

    var storeViewModel = {
        OpeningBalance: vndBalance,
        OpeningBalanceDollars: dollarBalance,
        OpeningBalanceYens: jpyBalance,
        OpeningCurrencyDetail: formattedString || "",
        OpeningCurrencyDetailDollars: formattedStringDollars || "",
        OpeningCurrencyDetailYens: formattedStringYens || "",
        StoreId: storeId || 0
    };

    $.ajax({
        url: '/Stores/OpenShop',
        type: 'POST',
        data: JSON.stringify(storeViewModel),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            if (response.Success) {
                window.location.href = '/SOSR/Create?IsReturn=false';
                $('#storeOpeningPopup').modal('hide');
            } else {
                alert('Error: ' + response.Message);
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred: ' + error);
        }
    });
}
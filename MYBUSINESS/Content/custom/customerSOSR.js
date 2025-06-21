var productColumns = [{ name: 'Id', minWidth: '50px' }, { name: 'Product', minWidth: '190px' }, { name: 'Sale Price', minWidth: '100px' }, { name: 'Stock', minWidth: '70px' }, { name: 'PerPack', minWidth: '100px' }, { name: 'W.S.P', minWidth: '100px' }];
//var products = []; //[['Ciplet', '10', '60'], ['Gaviscon', '85', '12'], ['Surficol', '110', '8']];
var products = new Array();

var customerColumns = [{ name: 'Code', minWidth: '100px' }, { name: 'CompanyName', minWidth: '170px' }, { name: 'Vat', minWidth: '100px' }];
//var customerColumns = [{ name: 'Id', minWidth: '100px' }, { name: 'Name', minWidth: '100px' }, { name: 'Address', minWidth: '160px' }, { name: 'Balance', minWidth: '100px' }, { name: 'Email', minWidth: '190px' }, { name: 'Vat', minWidth: '100px' }, { name: 'CompanyName', minWidth: '100px' }];
//var products = []; //[['Ciplet', '10', '60'], ['Gaviscon', '85', '12'], ['Surficol', '110', '8']];
var customers = new Array();
var productsBarcodes = new Array();
var productsBarcodess = [];
var _usdToVndRate = 0;
var _jpyToVndRate = 0;
var usdToVndRate = 0; // Exchange rate for USD to VND
var jpyToVndRate = 0; // Exchange rate for JPY to VND
//var usdToVndRate = 0; // Exchange rate for USD to VND
//var jpyToVndRate = 0; // Exchange rate for JPY to VND
//const usdToVndRate = 23600; // Exchange rate for USD to VND
//const jpyToVndRate = 179; // Exchange rate for JPY to VND
//console.log(productsBarcodes); // Check the contents of the array
//var productsBarcodess = new Array();
//var focusedBtnId = "";
//var focusedBtnSno = "";
var txtSerialNum = 0;
var clickedTextboxId = "name0";
var clickedIdNum = "";
var x, y;
var _total = 0;
var IsReturn = "false";
var num = 0;
//var storeIds = storeId;
//if (storeIds != null || storeIds != undefined) {
//    debugger;
//    document.getElementById('closeStore').style.display = 'block';
//    document.getElementById("StorageItem").value = storeIds;
//}
var availableCurrencies = [];
function getAll_AvailableCurrencies() {
    $.ajax({
        url: '/Currencies/GetAllAbvailableCurrencies',
        type: 'GET',
        success: function (response) {
            if (response.Success) {
                availableCurrencies = response.Data;
                _usdToVndRate = availableCurrencies.find(currency => currency.Name === 'USD');
                _jpyToVndRate = availableCurrencies.find(currency => currency.Name === 'JPY');
                usdToVndRate = _usdToVndRate.ExchangeRate;
                jpyToVndRate = _jpyToVndRate.ExchangeRate;
            } else {
                alert('Failed to set session: ' + response.Message);
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred while setting the session: ' + error);
        }
    });
}
function OnTypeCustomerName(param) {
    debugger;
    //debugger;
    $(param).mcautocomplete({
        showHeader: true,
        columns: customerColumns,
        source: customers,
        select: function (event, ui) {
            //debugger;
            this.value = (ui.item ? ui.item[1] : '');
            //productName = this.value;
            $('#vndCustomer').val(ui.item ? ui.item[1] : '');
            //$('#idnCustomer').val(ui.item ? ui.item[0] : '');
            //$('#vndCustomerCompany').val(ui.item ? ui.item[1] : '');
            //$('#vndCustomerVat').val(ui.item ? ui.item[2] : '');

            //$('#idnCustomer').val(ui.item ? ui.item[0] : '');
            //$('#customerAddress').val(ui.item ? ui.item[2] : '');
            //$('#vndCustomerName').val(ui.item ? ui.item[1] : '');
            //$('#vndCustomerAddress').val(ui.item ? ui.item[2] : '');
            //$('#vndCustomerEmail').val(ui.item ? ui.item[4] : '');
            //$('#vndCustomerVat').val(ui.item ? ui.item[5] : '');
            //$('#vndCustomerCompany').val(ui.item ? ui.item[6] : '');
            //$('#PreviousBalance').val(ui.item ? ui.item[3] : '');
            //update_itemTotal();
            //document.getElementById('name' + txtSerialNum).focus();
            return false;
        }

    });

}
var productName = "";
var pfound = 0;

function OnTypeName(param) {
    //alert(products);
    //alert(clickedTextboxId);
    //$('#name' + txtSerialNum).mcautocomplete({

    $(param).keyup(function (e) {

        clickedTextboxId = $(document.activeElement).attr("id");
        clickedIdNum = clickedTextboxId.substring(4);

    });

    $(param).mcautocomplete({
        showHeader: true,
        columns: productColumns,
        source: products,
        select: function (event, ui) {
            //debugger;
            var pfound = 0;

            // Decode HTML entities from a text
            function decodeHtmlEntities(text) {
                var textarea = document.createElement('textarea');
                textarea.innerHTML = text;
                return textarea.value;
            }

            // Decode ui.item properties
            var decodedId = decodeHtmlEntities(ui.item[0]);
            var decodedName = decodeHtmlEntities(ui.item[1]);
            var decodedSalePrice = decodeHtmlEntities(ui.item[2]);
            var decodedPerPack = decodeHtmlEntities(ui.item[4]);

            $('#selectedProducts > tbody  > tr').each(function () {
                if ($(this).find("[id^='idn']").val() == decodedId) {
                    var num = +$(this).find("[id^='quantity']").val() + 1;
                    $(this).find("[id^='quantity']").val(num);
                    update_itemTotal();
                    pfound = 1;
                    return false;
                }
            });

            if (pfound == 0) {
                this.value = decodedName;
                productName = decodedName;

                $('#name' + clickedIdNum).val(decodedName);
                $('#perPack' + clickedIdNum).val(decodedPerPack);
                $('#salePrice' + clickedIdNum).val(decodedSalePrice);
                $('#quantity' + clickedIdNum).val(1);
                $('#idn' + clickedIdNum).val(decodedId);

                update_itemTotal();
                return false;
            }
        }

        ////Old Code working commented by Sohail
        //showHeader: true,
        //columns: productColumns,
        //source: products,
        //select: function (event, ui) {
        //    debugger;
        //    pfound = 0;
        //    $('#selectedProducts > tbody  > tr').each(function () {

        //        if ($(this).find("[id^='idn']").val() == ui.item[0]) {
        //            num = + $(this).find("[id^='quantity']").val() + 1;
        //            $(this).find("[id^='quantity']").val(num);
        //            //alert($(this).find("[id^='quantity']").val());
        //            //$(this).find("[id^='quantity']").val() += 1;
        //            //alert(ui.item[0]);
        //            update_itemTotal();
        //            pfound = 1;
        //            return false;
        //        }
        //    })

        //    if (pfound == 0) {


        //        this.value = (ui.item ? ui.item[1] : '');
        //        productName = this.value;
        //        //if ($('#isPack' + clickedIdNum).val() == "true") {//false=piece true=PerPack
        //        $('#name' + clickedIdNum).val(ui.item ? ui.item[1] : '');
        //        $('#perPack' + clickedIdNum).val(ui.item ? ui.item[4] : '');
        //        //}

        //        $('#salePrice' + clickedIdNum).val(ui.item ? ui.item[2] : '');
        //        $('#quantity' + clickedIdNum).val(ui.item ? 1 : '');
        //        $('#idn' + clickedIdNum).val(ui.item ? ui.item[0] : '');
        //        //document.getElementById(clickedTextboxId).focus();
        //        update_itemTotal();
        //        //FetchProductRentStatus();
        //        return false;
        //    }
        //}
    });


    //alert("yes");
}

// Function to add a product to the table
//function addProduct(encodedProductJson) {
//    // Parse the JSON string into a JavaScript object
//    var product = JSON.parse(encodedProductJson);

//    // Search for the product in the table
//    var existingRow = findProductRow(product.Name);

//    if (existingRow) {
//        // Existing product logic
//        var qtyCell = existingRow.cells[3]; // Assuming quantity is the 4th cell (index 3)
//        var totalCell = existingRow.cells[4]; // Assuming total price is the 5th cell (index 4)
//        var currentQty = parseInt(qtyCell.innerText, 10);
//        var newQty = currentQty + 1;

//        qtyCell.innerText = newQty;
//        totalCell.innerText = (newQty * product.SalePrice).toFixed(2); // Update total price
//    } else {
//        // Create a new row if the product does not exist
//        var newRow = document.createElement('tr');
//        var indexCell = document.createElement('td');
//        var nameCell = document.createElement('td');
//        var priceCell = document.createElement('td');
//        var qtyCell = document.createElement('td');
//        var totalCell = document.createElement('td');
//        var actionsCell = document.createElement('td');

//        var rowIndex = document.querySelectorAll('#selectedProducts tbody tr').length; // Zero-based index

//        // Fill the cells with product data
//        indexCell.innerText = rowIndex + 1; // Row index starts from 1
//        nameCell.innerText = product.Name;
//        priceCell.innerText = product.SalePrice.toFixed(2);
//        qtyCell.innerText = 1;
//        totalCell.innerText = product.SalePrice.toFixed(2);

//        // Add an action button
//        var removeButton = document.createElement('button');
//        removeButton.innerText = 'Remove';
//        removeButton.type = 'button';
//        removeButton.className = 'btn btn-danger btn-sm';
//        removeButton.onclick = function () {
//            removeProduct(newRow, product.SalePrice);
//        };
//        actionsCell.appendChild(removeButton);

//        // Append the cells to the new row
//        newRow.appendChild(indexCell);
//        newRow.appendChild(nameCell);
//        newRow.appendChild(priceCell);
//        newRow.appendChild(qtyCell);
//        newRow.appendChild(totalCell);
//        newRow.appendChild(actionsCell);

//        // Append the new row to the table body
//        document.querySelector('#selectedProducts tbody').appendChild(newRow);

//        // Update row indices
//        updateRowIndices();
//    }

//    // Update the total amount for the "Pay" button
//    updatePayButton();
//}
var cardVndBalance;
var cashVndBalance;

var totalPayableBill;
var totalBillPaid;
var leftVndBalance;

var totalVndBalanceHeader = 0;

// Function to format numbers with dots as thousand separators without decimal places
function formatNumberWithDots(number) {
    var numberStr = Math.floor(number).toString();
    return numberStr.replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

//function addProduct(encodedProductJson) {
//    debugger;
//    // Parse the JSON string into a JavaScript object
//    var product = JSON.parse(encodedProductJson);
//    var rowIndex = document.querySelectorAll('#selectedProducts tbody tr').length; // Zero-based index

//    // Search for the product in the table
//    var existingRow = findProductRow(product.Name);

//    if (existingRow) {
//        // Existing product logic
//        var qtyCell = existingRow.cells[3]; // Assuming quantity is the 4th cell (index 3)
//        var totalCell = existingRow.cells[4]; // Assuming total price is the 5th cell (index 4)
//        var currentQty = parseInt(qtyCell.innerText, 10);
//        var newQty = currentQty + 1;

//        // Calculate the new total price
//        var newTotalPrice = newQty * product.SalePrice;
//        // Format the total price using the function
//        totalCell.innerText = formatNumberWithDots(newTotalPrice.toFixed(2));

//        //   qtyCell.innerText = newQty;
//        //totalCell.innerText = (newQty * product.SalePrice).toFixed(2); // Update total price

//        // Update hidden inputs
//        var hiddenQtyInput = existingRow.querySelector("[name^='SaleOrderDetail'][name$='Quantity']");
//        hiddenQtyInput.value = newQty;

//    } else {
//        // Create a new row if the product does not exist
//        var newRow = document.createElement('tr');
//        var indexCell = document.createElement('td');
//        var nameCell = document.createElement('td');
//        var priceCell = document.createElement('td');
//        var qtyCell = document.createElement('td');
//        var totalCell = document.createElement('td');
//        var actionsCell = document.createElement('td');

//        // Fill the cells with product data
//        indexCell.innerText = rowIndex + 1; // Row index starts from 1
//        nameCell.innerText = product.Name;
//        priceCell.innerText = product.SalePrice.toFixed(2);
//        qtyCell.innerText = 1;
//        totalCell.innerText = product.SalePrice.toFixed(2);



//        // Create hidden inputs
//        var hiddenProductIdInput = document.createElement('input');
//        hiddenProductIdInput.type = 'hidden';
//        hiddenProductIdInput.name = 'SaleOrderDetail[' + rowIndex + '].ProductId';
//        //hiddenProductIdInput.value = product.ProductId; // Adjust based on your JSON structure
//        hiddenProductIdInput.value = product.Id; // Adjust based on your JSON structure

//        var hiddenProductNameInput = document.createElement('input');
//        hiddenProductNameInput.type = 'hidden';
//        hiddenProductNameInput.name = 'SaleOrderDetail[' + rowIndex + '].Product.Name';
//        //hiddenProductIdInput.value = product.ProductId; // Adjust based on your JSON structure
//        hiddenProductNameInput.value = product.Name; // Adjust based on your JSON structure

//        var hiddenSalePriceInput = document.createElement('input');
//        hiddenSalePriceInput.type = 'hidden';
//        hiddenSalePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].SalePrice';
//        hiddenSalePriceInput.value = product.SalePrice;

//        var hiddenPurchasePriceInput = document.createElement('input');
//        hiddenPurchasePriceInput.type = 'hidden';
//        hiddenPurchasePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].PurchasePrice';
//        hiddenPurchasePriceInput.value = product.PurchasePrice;

//        var hiddenQuantityInput = document.createElement('input');
//        hiddenQuantityInput.type = 'hidden';
//        hiddenQuantityInput.name = 'SaleOrderDetail[' + rowIndex + '].Quantity';
//        hiddenQuantityInput.value = 1;

//        // Set the value formatted to two decimal places SasleOrder form
//        //updateSalesForm(product.SalePrice)
//        //$('#ItemsTotal').val(parseFloat(hiddenSalePriceInput.value).toFixed(2));
//        //
//        // Add an action button
//        var removeButton = document.createElement('button');
//        //removeButton.innerText = 'Remove';
//        removeButton.type = 'button';
//        //removeButton.className = 'btn btn-danger btn-sm';
//        removeButton.className = 'btn btn-sm';
//        // Create the icon element
//        var icon = document.createElement('i');
//        icon.className = 'material-icons'; // Material Icons class
//        icon.innerText = 'delete'; // Material Icons name for the trash icon
//        icon.style.color = '#FF6F6F';
//        // Append the icon to the button
//        removeButton.appendChild(icon);
//        removeButton.onclick = function () {
//            removeProduct(newRow, product.SalePrice);
//        };
//        actionsCell.appendChild(removeButton);

//        // Append the cells to the new row
//        newRow.appendChild(indexCell);
//        newRow.appendChild(nameCell);
//        newRow.appendChild(priceCell);
//        newRow.appendChild(qtyCell);
//        newRow.appendChild(totalCell);
//        newRow.appendChild(actionsCell);

//        // Append hidden inputs to the row
//        newRow.appendChild(hiddenProductIdInput);
//        newRow.appendChild(hiddenProductNameInput);
//        newRow.appendChild(hiddenSalePriceInput);
//        newRow.appendChild(hiddenPurchasePriceInput);
//        newRow.appendChild(hiddenQuantityInput);

//        // Append the new row to the table body
//        document.querySelector('#selectedProducts tbody').appendChild(newRow);

//        // Update row indices
//        updateRowIndices();
//    }

//    // Update the total amount for the "Pay" button
//    updatePayButton();
//}
//function addProduct(encodedProductJson) {
//    debugger;
//    // Parse the JSON string into a JavaScript object
//    var product = JSON.parse(encodedProductJson);
//    var rowIndex = document.querySelectorAll('#selectedProducts tbody tr').length; // Zero-based index
//    // Retrieve the SuggestedNewProductId value from the hidden input field
//    var suggestedNewProductId = document.getElementById('suggestedNewProductId').value;
//    // Search for the product in the table
//    var existingRow = findProductRow(product.Name);

//    if (existingRow) {
//        // Existing product logic
//        var qtyCell = existingRow.cells[3]; // Assuming quantity is the 4th cell (index 3)
//        var totalCell = existingRow.cells[4]; // Assuming total price is the 5th cell (index 4)
//        var currentQty = parseInt(qtyCell.innerText, 10);
//        var newQty = currentQty + 1;

//        // Calculate the new total price
//        var newTotalPrice = newQty * product.SalePrice;

//        // Format the total price using the custom function
//        totalCell.innerText = formatNumberWithDots(newTotalPrice);

//        totalVndBalanceHeader = formatNumberWithDots(newTotalPrice);
//        // Update quantity
//        qtyCell.innerText = newQty;

//        // Update hidden inputs
//        var hiddenQtyInput = existingRow.querySelector("[name^='SaleOrderDetail'][name$='Quantity']");
//        hiddenQtyInput.value = newQty;

//    } else {
//        // Create a new row if the product does not exist
//        var newRow = document.createElement('tr');
//        var indexCell = document.createElement('td');
//        var nameCell = document.createElement('td');
//        var priceCell = document.createElement('td');
//        var qtyCell = document.createElement('td');
//        var totalCell = document.createElement('td');
//        var actionsCell = document.createElement('td');

//        // Fill the cells with product data
//        indexCell.innerText = rowIndex + 1; // Row index starts from 1
//        nameCell.innerText = product.Name;
//        // Format the SalePrice using the custom function
//        priceCell.innerText = formatNumberWithDots(product.SalePrice);
//        qtyCell.innerText = 1;
//        // Format the SalePrice again for the total since it's a single quantity
//        totalCell.innerText = formatNumberWithDots(product.SalePrice);
//        totalVndBalanceHeader = formatNumberWithDots(product.SalePrice);

//        // Create hidden inputs
//        var hiddenProductIdInput = document.createElement('input');
//        hiddenProductIdInput.type = 'hidden';
//        hiddenProductIdInput.name = 'SaleOrderDetail[' + rowIndex + '].ProductId';
//        hiddenProductIdInput.value = product.Id; // Adjust based on your JSON structure

//        var hiddenProductNameInput = document.createElement('input');
//        hiddenProductNameInput.type = 'hidden';
//        hiddenProductNameInput.name = 'SaleOrderDetail[' + rowIndex + '].Product.Name';
//        hiddenProductNameInput.value = product.Name; // Adjust based on your JSON structure

//        var hiddenSalePriceInput = document.createElement('input');
//        hiddenSalePriceInput.type = 'hidden';
//        hiddenSalePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].SalePrice';
//        hiddenSalePriceInput.value = product.SalePrice;

//        var hiddenPurchasePriceInput = document.createElement('input');
//        hiddenPurchasePriceInput.type = 'hidden';
//        hiddenPurchasePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].PurchasePrice';
//        hiddenPurchasePriceInput.value = product.PurchasePrice;

//        var hiddenQuantityInput = document.createElement('input');
//        hiddenQuantityInput.type = 'hidden';
//        hiddenQuantityInput.name = 'SaleOrderDetail[' + rowIndex + '].Quantity';
//        hiddenQuantityInput.value = 1;

//        // Add an action button
//        var removeButton = document.createElement('button');
//        removeButton.type = 'button';
//        removeButton.className = 'btn btn-sm';
//        // Create the icon element
//        var icon = document.createElement('i');
//        icon.className = 'material-icons'; // Material Icons class
//        icon.innerText = 'delete'; // Material Icons name for the trash icon
//        icon.style.color = '#FF6F6F';
//        // Append the icon to the button
//        removeButton.appendChild(icon);
//        removeButton.onclick = function () {
//            removeProduct(newRow, product.SalePrice);
//        };
//        actionsCell.appendChild(removeButton);

//        // Append the cells to the new row
//        newRow.appendChild(indexCell);
//        newRow.appendChild(nameCell);
//        newRow.appendChild(priceCell);
//        newRow.appendChild(qtyCell);
//        newRow.appendChild(totalCell);
//        newRow.appendChild(actionsCell);

//        // Append hidden inputs to the row
//        newRow.appendChild(hiddenProductIdInput);
//        newRow.appendChild(hiddenProductNameInput);
//        newRow.appendChild(hiddenSalePriceInput);
//        newRow.appendChild(hiddenPurchasePriceInput);
//        newRow.appendChild(hiddenQuantityInput);

//        // Append the new row to the table body
//        document.querySelector('#selectedProducts tbody').appendChild(newRow);

//        // Update row indices
//        updateRowIndices();
//    }

//    // Update the total amount for the "Pay" button
//    updatePayButton();
//}
//function addProduct(encodedProductJson) {
//    debugger;
//    var quantityFromModal = 0;
//    $('#quantityAddpopup').modal('show');
//    $('#validateQuantity').click(function (e) {
//        quantityFromModal = $('#quantityPopup').val();
//    });
//    // Parse the JSON string into a JavaScript object
//    var product = JSON.parse(encodedProductJson);
//    var rowIndex = document.querySelectorAll('#selectedProducts tbody tr').length; // Zero-based index

//    // Retrieve the SuggestedNewProductId value from the hidden input field
//    var suggestedNewProductId = document.getElementById('suggestedNewProductId').value;

//    // Search for the product in the table
//    var existingRow = findProductRow(product.Name);

//    if (existingRow) {
//        // Existing product logic
//        var qtyCell = existingRow.cells[3]; // Assuming quantity is the 4th cell (index 3)
//        var totalCell = existingRow.cells[5]; // Assuming total price is the 6th cell (index 5)
//        var currentQty = parseInt(qtyCell.innerText, 10);
//        var newQty = currentQty + 1;

//        // Calculate the new total price
//        var newTotalPrice = newQty * product.SalePrice;

//        // Format the total price using the custom function
//        totalCell.innerText = formatNumberWithDots(newTotalPrice);

//        // Update quantity
//        qtyCell.innerText = newQty;

//        // Update hidden inputs
//        var hiddenQtyInput = existingRow.querySelector("[name^='SaleOrderDetail'][name$='Quantity']");
//        hiddenQtyInput.value = newQty;

//        // Update the totalVndBalanceHeader by adding the difference in price
//        var oldTotalPrice = currentQty * product.SalePrice;
//        totalVndBalanceHeader += newTotalPrice - oldTotalPrice;
//    } else {
//        // Create a new row if the product does not exist
//        var newRow = document.createElement('tr');
//        var indexCell = document.createElement('td');
//        var nameCell = document.createElement('td');
//        var priceCell = document.createElement('td');
//        var qtyCell = document.createElement('td');
//        var billNumberCell = document.createElement('td'); // Bill Number Cell
//        var totalCell = document.createElement('td');
//        var actionsCell = document.createElement('td');

//        // Fill the cells with product data
//        indexCell.innerText = rowIndex + 1; // Row index starts from 1
//        nameCell.innerText = product.Name;

//        // Format the SalePrice using the custom function
//        priceCell.innerText = formatNumberWithDots(product.SalePrice);
//        qtyCell.innerText = 1;

//        // Directly set Bill Number to SuggestedNewProductId (do not perform calculations on it)
//        billNumberCell.innerText = suggestedNewProductId;

//        // Calculate total based on SalePrice and quantity (no relation to Bill Number)
//        totalCell.innerText = formatNumberWithDots(product.SalePrice);

//        // Accumulate the total price to totalVndBalanceHeader
//        totalVndBalanceHeader += product.SalePrice;

//        // Create hidden inputs
//        var hiddenProductIdInput = document.createElement('input');
//        hiddenProductIdInput.type = 'hidden';
//        hiddenProductIdInput.name = 'SaleOrderDetail[' + rowIndex + '].ProductId';
//        hiddenProductIdInput.value = product.Id;

//        var hiddenProductNameInput = document.createElement('input');
//        hiddenProductNameInput.type = 'hidden';
//        hiddenProductNameInput.name = 'SaleOrderDetail[' + rowIndex + '].Product.Name';
//        hiddenProductNameInput.value = product.Name;

//        var hiddenSalePriceInput = document.createElement('input');
//        hiddenSalePriceInput.type = 'hidden';
//        hiddenSalePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].SalePrice';
//        hiddenSalePriceInput.value = product.SalePrice;

//        var hiddenPurchasePriceInput = document.createElement('input');
//        hiddenPurchasePriceInput.type = 'hidden';
//        hiddenPurchasePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].PurchasePrice';
//        hiddenPurchasePriceInput.value = product.PurchasePrice;

//        var hiddenQuantityInput = document.createElement('input');
//        hiddenQuantityInput.type = 'hidden';
//        hiddenQuantityInput.name = 'SaleOrderDetail[' + rowIndex + '].Quantity';
//        hiddenQuantityInput.value = 1;

//        // Add an action button
//        var removeButton = document.createElement('button');
//        removeButton.type = 'button';
//        removeButton.className = 'btn btn-sm';
//        var icon = document.createElement('i');
//        icon.className = 'material-icons';
//        icon.innerText = 'delete';
//        icon.style.color = '#FF6F6F';
//        removeButton.appendChild(icon);
//        removeButton.onclick = function () {
//            removeProduct(newRow, product.SalePrice);
//        };
//        actionsCell.appendChild(removeButton);

//        // Append the cells to the new row
//        newRow.appendChild(indexCell);
//        newRow.appendChild(nameCell);
//        newRow.appendChild(priceCell);
//        newRow.appendChild(qtyCell);
//        newRow.appendChild(billNumberCell); // Append Bill Number Cell
//        newRow.appendChild(totalCell);
//        newRow.appendChild(actionsCell);

//        // Append hidden inputs to the row
//        newRow.appendChild(hiddenProductIdInput);
//        newRow.appendChild(hiddenProductNameInput);
//        newRow.appendChild(hiddenSalePriceInput);
//        newRow.appendChild(hiddenPurchasePriceInput);
//        newRow.appendChild(hiddenQuantityInput);

//        // Append the new row to the table body
//        document.querySelector('#selectedProducts tbody').appendChild(newRow);

//        // Update row indices
//        updateRowIndices();
//    }

//    // Update the total amount for the "Pay" button
//    updatePayButton();
//}
let focusedInput = null;
let isFirstEntry = true;
var isPupdateQuantity = false;
function validateQuantityInput(event) {
    debugger;
    // Allow only numeric values
    const value = event.target.value;

    // Remove non-numeric characters
    if (!/^\d*$/.test(value)) {
        event.target.value = value.replace(/\D/g, '');
    }

    // Limit to 10 digits
    if (value.length > 10) {
        event.target.value = value.slice(0, 10);
    }
    // Ensure the value is not equal to 0 or empty
    if (event.target.value === '0' || event.target.value === '') {
        event.target.value = '';  // Default to 1 if 0 or empty is entered
    }
    //if (event.target.value === '0' || event.target.value === '') {
    //    event.target.value = '1';  // Default to 1 if 0 or empty is entered
    //}
}
function addProduct(encodedProductJson) {
    debugger;

    // Show the modal and set up the event listener for quantity validation
    $('#quantityAddpopup').modal('show');
    $('#quantityAddpopup').on('shown.bs.modal', function () {
        //$('#productQuantity').focus().select();
        $('#productQuantity').focus();
    });
    // Highlight text and reset flag when input gains focus
    $('#productQuantity').on('focus', function () {
        isFirstEntry = true; // Reset flag on focus
        focusedInput = $(this); // Set focusedInput to the current input
        $(this).select(); // Highlight the text in the input
    });
    //// Highlight text when input gains focus
    //$('#productQuantity').on('focus', function () {
    //    isFirstEntry = true; // Reset flag on focus
    //    $(this).select(); // Highlight the text in the input
    //});

    //// Assuming focusedInput is set when the input gains focus
    //$('#productQuantity').on('focus', function () {
    //    focusedInput = $(this); // Set focusedInput to the current input
    //});

    //$('#validateQuantity').off('click').on('click', function (e) { //////Working code
    //    debugger;
    //    var quantityFromModal = parseInt($('#productQuantity').val(), 10) || 0; // Ensure it's a number and default to 0 if invalid
    //    if (quantityFromModal == 0) {
    //        return;
    //    }
    //    $('#quantityAddpopup').modal('hide'); // Hide the modal
    //    $('#productQuantity').val(1);
    //    //$('#productQuantity').val('');
    //    //After getting the quantity, call the function to add the product
    //    handleProductAddition(encodedProductJson, quantityFromModal, true);
    //});
    $('#validateQuantity').off('click').on('click', function (e) {
        debugger;
        var quantityFromModal = parseInt($('#productQuantity').val(), 10) || 0; // Ensure it's a number and default to 0 if invalid
        if (quantityFromModal == 0) {
            return;
        }

        // Hide the modal and reset the input
        $('#quantityAddpopup').modal('hide');
        $('#productQuantity').val(1);

        // Get the product name stored in the modal
        if (isPupdateQuantity) {
            var productJsonString = $('#quantityAddpopup').data('productDetails');
            if (productJsonString != undefined) {
                var product = JSON.parse(productJsonString);
                if (product != undefined) {
                    var existingRow = findProductRow(product.Name);
                    if (existingRow) {
                        debugger;
                        // If the product row exists, update it directly
                        handleProductAddition(productJsonString, quantityFromModal, true);
                        isPupdateQuantity = false;
                    }
                    else {
                        return;
                    }
                }
                else {
                    handleProductAddition(encodedProductJson, quantityFromModal, false);
                }
            }
            else { handleProductAddition(encodedProductJson, quantityFromModal, false); }
        }
        else { debugger; handleProductAddition(encodedProductJson, quantityFromModal, false); }
        // Find the row for the existing product based on the product name

    });

}
function findProductRow(productName) {
    debugger
    var rows = document.querySelectorAll('#selectedProducts tbody tr');
    for (var i = 0; i < rows.length; i++) {
        var nameCell = rows[i].cells[1]; // Assuming the product name is in the second cell
        if (nameCell.innerText === productName) {
            return rows[i];
        }
    }
    return null;
}
//function handleProductAddition(encodedProductJson, quantityFromModal) {
//    debugger;
//    // Parse the JSON string into a JavaScript object
//    var product = JSON.parse(encodedProductJson);
//    var rowIndex = document.querySelectorAll('#selectedProducts tbody tr').length; // Zero-based index

//    // Retrieve the SuggestedNewProductId value from the hidden input field
//    var suggestedNewProductId = document.getElementById('suggestedNewProductId').value;

//    // Search for the product in the table
//    var existingRow = findProductRow(product.Name);

//    if (existingRow) {
//        // Existing product logic
//        var qtyCell = existingRow.cells[3]; // Assuming quantity is the 4th cell (index 3)
//        var totalCell = existingRow.cells[5]; // Assuming total price is the 6th cell (index 5)
//        var currentQty = parseInt(qtyCell.innerText, 10);
//        var newQty = currentQty + quantityFromModal;

//        // Calculate the new total price
//        var newTotalPrice = newQty * product.SalePrice;

//        // Format the total price using the custom function
//        totalCell.innerText = formatNumberWithDots(newTotalPrice);

//        // Update quantity
//        qtyCell.innerText = newQty;

//        // Update hidden inputs
//        var hiddenQtyInput = existingRow.querySelector("[name^='SaleOrderDetail'][name$='Quantity']");
//        hiddenQtyInput.value = newQty;

//        // Update the totalVndBalanceHeader by adding the difference in price
//        var oldTotalPrice = currentQty * product.SalePrice;
//        totalVndBalanceHeader += newTotalPrice - oldTotalPrice;
//    } else {
//        // Create a new row if the product does not exist
//        var newRow = document.createElement('tr');
//        var indexCell = document.createElement('td');
//        var nameCell = document.createElement('td');
//        var priceCell = document.createElement('td');
//        var qtyCell = document.createElement('td');
//        var billNumberCell = document.createElement('td'); // Bill Number Cell
//        var totalCell = document.createElement('td');
//        totalCell.name = 'selectedProductTotal';
//        totalCell.id = 'selectedProductTotal';
//        var actionsCell = document.createElement('td');

//        // Fill the cells with product data
//        indexCell.innerText = rowIndex + 1; // Row index starts from 1
//        nameCell.innerText = product.Name;

//        // Format the SalePrice using the custom function
//        priceCell.innerText = formatNumberWithDots(product.SalePrice);
//        qtyCell.innerText = quantityFromModal;

//        // Directly set Bill Number to SuggestedNewProductId (do not perform calculations on it)
//        billNumberCell.innerText = suggestedNewProductId;

//        // Calculate total based on SalePrice and quantity
//        var totalAmount = quantityFromModal * product.SalePrice;
//        totalCell.innerText = formatNumberWithDots(totalAmount);

//        // Accumulate the total price to totalVndBalanceHeader
//        totalVndBalanceHeader += totalAmount;

//        // Create hidden inputs
//        var hiddenProductIdInput = document.createElement('input');
//        hiddenProductIdInput.type = 'hidden';
//        hiddenProductIdInput.name = 'SaleOrderDetail[' + rowIndex + '].ProductId';
//        hiddenProductIdInput.value = product.Id;

//        var hiddenProductNameInput = document.createElement('input');
//        hiddenProductNameInput.type = 'hidden';
//        hiddenProductNameInput.name = 'SaleOrderDetail[' + rowIndex + '].Product.Name';
//        hiddenProductNameInput.value = product.Name;

//        var hiddenSalePriceInput = document.createElement('input');
//        hiddenSalePriceInput.type = 'hidden';
//        hiddenSalePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].SalePrice';
//        hiddenSalePriceInput.value = product.SalePrice;

//        var hiddenPurchasePriceInput = document.createElement('input');
//        hiddenPurchasePriceInput.type = 'hidden';
//        hiddenPurchasePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].PurchasePrice';
//        hiddenPurchasePriceInput.value = product.PurchasePrice;

//        var hiddenQuantityInput = document.createElement('input');
//        hiddenQuantityInput.type = 'hidden';
//        hiddenQuantityInput.name = 'SaleOrderDetail[' + rowIndex + '].Quantity';
//        hiddenQuantityInput.value = quantityFromModal;

//        // Add an action button
//        var removeButton = document.createElement('button');
//        removeButton.type = 'button';
//        removeButton.className = 'btn btn-sm';
//        var icon = document.createElement('i');
//        icon.className = 'material-icons';
//        icon.innerText = 'delete';
//        icon.style.color = '#FF6F6F';
//        removeButton.appendChild(icon);
//        removeButton.onclick = function () {
//            removeProduct(newRow, product.SalePrice);
//        };
//        actionsCell.appendChild(removeButton);

//        // Append the cells to the new row
//        newRow.appendChild(indexCell);
//        newRow.appendChild(nameCell);
//        newRow.appendChild(priceCell);
//        newRow.appendChild(qtyCell);
//        newRow.appendChild(billNumberCell); // Append Bill Number Cell
//        newRow.appendChild(totalCell);
//        newRow.appendChild(actionsCell);

//        // Append hidden inputs to the row
//        newRow.appendChild(hiddenProductIdInput);
//        newRow.appendChild(hiddenProductNameInput);
//        newRow.appendChild(hiddenSalePriceInput);
//        newRow.appendChild(hiddenPurchasePriceInput);
//        newRow.appendChild(hiddenQuantityInput);

//        // Append the new row to the table body
//        document.querySelector('#selectedProducts tbody').appendChild(newRow);

//        // Update row indices
//        updateRowIndices();
//    }

//    // Update the total amount for the "Pay" button
//    updatePayButton();
//}
//function handleProductAddition(encodedProductJson, quantityFromModal) {
//    debugger;
//    // Parse the JSON string into a JavaScript object
//    var product = JSON.parse(encodedProductJson);
//    var rowIndex = document.querySelectorAll('#selectedProducts tbody tr').length; // Zero-based index

//    // Retrieve the SuggestedNewProductId value from the hidden input field
//    var suggestedNewProductId = document.getElementById('suggestedNewProductId').value;

//    // Search for the product in the table
//    var existingRow = findProductRow(product.Name);

//    if (existingRow) {
//        // Existing product logic
//        var qtyCell = existingRow.cells[3]; // Assuming quantity is the 4th cell (index 3)
//        var totalCell = existingRow.cells[5]; // Assuming total price is the 6th cell (index 5)
//        var currentQty = parseInt(qtyCell.innerText, 10);
//        var newQty = currentQty + quantityFromModal;

//        // Calculate the new total price
//        var newTotalPrice = newQty * product.SalePrice;

//        // Format the total price using the custom function
//        totalCell.innerText = formatNumberWithDots(newTotalPrice);

//        // Update quantity
//        qtyCell.innerText = newQty;

//        // Update hidden inputs
//        var hiddenQtyInput = existingRow.querySelector("[name^='SaleOrderDetail'][name$='Quantity']");
//        hiddenQtyInput.value = newQty;

//        // Update the totalVndBalanceHeader by adding the difference in price
//        var oldTotalPrice = currentQty * product.SalePrice;
//        totalVndBalanceHeader += newTotalPrice - oldTotalPrice;
//    } else {
//        // Create a new row if the product does not exist
//        var newRow = document.createElement('tr');
//        var indexCell = document.createElement('td');
//        var nameCell = document.createElement('td');
//        var priceCell = document.createElement('td');
//        var qtyCell = document.createElement('td');
//        var billNumberCell = document.createElement('td'); // Bill Number Cell
//        var totalCell = document.createElement('td');
//        totalCell.name = 'selectedProductTotal';
//        totalCell.id = 'selectedProductTotal' + rowIndex; // Make sure ID is unique per row
//        var actionsCell = document.createElement('td');

//        // Fill the cells with product data
//        indexCell.innerText = rowIndex + 1; // Row index starts from 1
//        nameCell.innerText = product.Name;

//        // Format the SalePrice using the custom function
//        priceCell.innerText = formatNumberWithDots(product.SalePrice);

//        //Hide Price cell
//        priceCell.style.display = 'none';

//        qtyCell.innerText = quantityFromModal;

//        // Directly set Bill Number to SuggestedNewProductId (do not perform calculations on it)
//        billNumberCell.innerText = suggestedNewProductId;
//        //Hide bill NumberCell cell
//        billNumberCell.style.display = 'none';

//        // Calculate total based on SalePrice and quantity
//        var totalAmount = quantityFromModal * product.SalePrice;
//        totalCell.innerText = formatNumberWithDots(totalAmount);

//        // Accumulate the total price to totalVndBalanceHeader
//        totalVndBalanceHeader += totalAmount;

//        // Create hidden inputs
//        var hiddenProductIdInput = document.createElement('input');
//        hiddenProductIdInput.type = 'hidden';
//        hiddenProductIdInput.name = 'SaleOrderDetail[' + rowIndex + '].ProductId';
//        hiddenProductIdInput.value = product.Id;

//        var hiddenProductNameInput = document.createElement('input');
//        hiddenProductNameInput.type = 'hidden';
//        hiddenProductNameInput.name = 'SaleOrderDetail[' + rowIndex + '].Product.Name';
//        hiddenProductNameInput.value = product.Name;

//        var hiddenSalePriceInput = document.createElement('input');
//        hiddenSalePriceInput.type = 'hidden';
//        hiddenSalePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].SalePrice';
//        hiddenSalePriceInput.value = product.SalePrice;

//        var hiddenPurchasePriceInput = document.createElement('input');
//        hiddenPurchasePriceInput.type = 'hidden';
//        hiddenPurchasePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].PurchasePrice';
//        hiddenPurchasePriceInput.value = product.PurchasePrice;

//        var hiddenQuantityInput = document.createElement('input');
//        hiddenQuantityInput.type = 'hidden';
//        hiddenQuantityInput.name = 'SaleOrderDetail[' + rowIndex + '].Quantity';
//        hiddenQuantityInput.value = quantityFromModal;

//        //var hiddenSaleTypeInput = document.createElement('input');
//        //hiddenSaleTypeInput.type = 'hidden';
//        //hiddenSaleTypeInput.name = 'SaleOrderDetail[' + rowIndex + '].SaleType';
//        //hiddenSaleTypeInput.value = product.SaleType;
//        //hiddenSaleTypeInput.value = 'true';

//        // Add an action button
//        var removeButton = document.createElement('button');
//        removeButton.type = 'button';
//        removeButton.className = 'btn btn-sm';
//        var icon = document.createElement('i');
//        icon.className = 'material-icons';
//        icon.innerText = 'delete';
//        icon.style.color = '#FF6F6F';
//        removeButton.appendChild(icon);
//        removeButton.onclick = function () {
//            removeProduct(newRow, product.SalePrice);
//        };
//        actionsCell.appendChild(removeButton);

//        // Append the cells to the new row
//        newRow.appendChild(indexCell);
//        newRow.appendChild(nameCell);
//        newRow.appendChild(priceCell);
//        newRow.appendChild(qtyCell);
//        newRow.appendChild(billNumberCell); // Append Bill Number Cell
//        newRow.appendChild(totalCell);
//        newRow.appendChild(actionsCell);

//        // Append hidden inputs to the row
//        newRow.appendChild(hiddenProductIdInput);
//        newRow.appendChild(hiddenProductNameInput);
//        newRow.appendChild(hiddenSalePriceInput);
//        newRow.appendChild(hiddenPurchasePriceInput);
//        newRow.appendChild(hiddenQuantityInput);
//        //newRow.appendChild(hiddenSaleTypeInput);

//        // Append the new row to the table body
//        document.querySelector('#selectedProducts tbody').appendChild(newRow);

//        // Update row indices
//        updateRowIndices();
//    }

//    // Update the total amount for the "Pay" button
//    updatePayButton();
//}
//function handleProductAddition(encodedProductJson, quantityFromScanner, isUpdate = false) {
//    debugger;
//    var product = JSON.parse(encodedProductJson);
//    var rowIndex = document.querySelectorAll('#selectedProducts tbody tr').length; // Zero-based index
//    var suggestedNewProductId = document.getElementById('suggestedNewProductId').value;
//    var existingRow = findProductRow(product.Name);

//    if (existingRow) {
//        // Existing product logic
//        var qtyInput = existingRow.cells[3].querySelector('input'); // Select input from qtyCell
//        var totalCell = existingRow.cells[5]; // Assuming total price is the 6th cell (index 5)
//        var currentQty = parseInt(qtyInput.value, 10);
//        //var newQty = currentQty + quantityFromScanner; //Working code without update case from quantity popu
//        var newQty;
//        ////Uddate quantity
//        if (isUpdate) {
//            // If it's an update, set the quantity from the modal directly
//            newQty = quantityFromScanner;
//        } else {
//            // If it's an addition, increment by the new quantity
//            var currentQty = parseInt(qtyInput.value, 10);
//            newQty = currentQty + quantityFromScanner;
//        }
//        /////
//        // Calculate the new total price
//        var newTotalPrice = newQty * product.SalePrice;
//        totalCell.innerText = formatNumberWithDots(newTotalPrice);

//        // Update quantity input
//        qtyInput.value = newQty;

//        // Update hidden inputs
//        var hiddenQtyInput = existingRow.querySelector("[name^='SaleOrderDetail'][name$='Quantity']");
//        hiddenQtyInput.value = newQty;

//        // Update the totalVndBalanceHeader by adding the difference in price
//        var oldTotalPrice = currentQty * product.SalePrice;
//        totalVndBalanceHeader += newTotalPrice - oldTotalPrice;
//    } else {
//        // Create a new row if the product does not exist
//        var newRow = document.createElement('tr');
//        var indexCell = document.createElement('td');
//        var nameCell = document.createElement('td');
//        var priceCell = document.createElement('td');
//        var qtyCell = document.createElement('td');
//        var billNumberCell = document.createElement('td');
//        var totalCell = document.createElement('td');
//        totalCell.name = 'selectedProductTotal';
//        totalCell.id = 'selectedProductTotal' + rowIndex;
//        var actionsCell = document.createElement('td');

//        // Fill the cells with product data
//        indexCell.innerText = rowIndex + 1; // Row index starts from 1
//        nameCell.innerText = product.Name;

//        priceCell.innerText = formatNumberWithDots(product.SalePrice);
//        priceCell.style.display = 'none'; // Hide Price cell

//        // Create an editable input for quantity
//        var qtyInput = document.createElement('input');
//        qtyInput.type = 'number';
//        qtyInput.min = '1';
//        qtyInput.value = quantityFromScanner;
//        qtyInput.className = 'form-control';
//        qtyInput.style.width = '60px'; // Adjust width as needed

//        // Listen for changes in the quantity input (on key press)
//        qtyInput.addEventListener('input', function () {
//            var newQty = parseInt(qtyInput.value, 10);
//            if (isNaN(newQty) || newQty < 1) {
//                newQty = 1; // Prevent invalid or negative values
//                qtyInput.value = 1;
//            }

//            // Calculate the new total price
//            var newTotalPrice = newQty * product.SalePrice;
//            totalCell.innerText = formatNumberWithDots(newTotalPrice);

//            // Update hidden quantity input
//            hiddenQuantityInput.value = newQty;

//            // Recalculate total balance
//            //updateTotalVndBalance();
//            updatePayButton();
//        });
//        qtyInput.addEventListener('click', function () {
//            $('#quantityAddpopup').modal('show');
//            $('#quantityAddpopup').data('productName', product.Name); // Example: store the product name
//            //removeProductsEntireRow(newRow, product.SalePrice);
//        });
//        // Append the input to the qtyCell
//        qtyCell.appendChild(qtyInput);

//        billNumberCell.innerText = suggestedNewProductId;
//        billNumberCell.style.display = 'none'; // Hide bill number cell

//        var totalAmount = quantityFromScanner * product.SalePrice;
//        totalCell.innerText = formatNumberWithDots(totalAmount);

//        totalVndBalanceHeader += totalAmount;

//        // Create hidden inputs
//        var hiddenProductIdInput = document.createElement('input');
//        hiddenProductIdInput.type = 'hidden';
//        hiddenProductIdInput.name = 'SaleOrderDetail[' + rowIndex + '].ProductId';
//        hiddenProductIdInput.value = product.Id;

//        var hiddenProductNameInput = document.createElement('input');
//        hiddenProductNameInput.type = 'hidden';
//        hiddenProductNameInput.name = 'SaleOrderDetail[' + rowIndex + '].Product.Name';
//        hiddenProductNameInput.value = product.Name;

//        var hiddenSalePriceInput = document.createElement('input');
//        hiddenSalePriceInput.type = 'hidden';
//        hiddenSalePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].SalePrice';
//        hiddenSalePriceInput.value = product.SalePrice;

//        var hiddenPurchasePriceInput = document.createElement('input');
//        hiddenPurchasePriceInput.type = 'hidden';
//        hiddenPurchasePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].PurchasePrice';
//        hiddenPurchasePriceInput.value = product.PurchasePrice;

//        var hiddenQuantityInput = document.createElement('input');
//        hiddenQuantityInput.type = 'hidden';
//        hiddenQuantityInput.name = 'SaleOrderDetail[' + rowIndex + '].Quantity';
//        hiddenQuantityInput.value = quantityFromScanner;

//        // Add action button for row deletion
//        var removeButton = document.createElement('button');
//        removeButton.type = 'button';
//        removeButton.className = 'btn btn-sm';
//        var icon = document.createElement('i');
//        icon.className = 'material-icons';
//        icon.innerText = 'delete';
//        icon.style.color = '#FF6F6F';
//        removeButton.appendChild(icon);
//        removeButton.onclick = function () {
//            removeProductsEntireRow(newRow, product.SalePrice);
//        };
//        actionsCell.appendChild(removeButton);

//        // Append the cells to the new row
//        newRow.appendChild(indexCell);
//        newRow.appendChild(nameCell);
//        newRow.appendChild(priceCell);
//        newRow.appendChild(qtyCell);
//        newRow.appendChild(billNumberCell);
//        newRow.appendChild(totalCell);
//        newRow.appendChild(actionsCell);

//        // Append hidden inputs to the row
//        newRow.appendChild(hiddenProductIdInput);
//        newRow.appendChild(hiddenProductNameInput);
//        newRow.appendChild(hiddenSalePriceInput);
//        newRow.appendChild(hiddenPurchasePriceInput);
//        newRow.appendChild(hiddenQuantityInput);

//        // Append the new row to the table body
//        document.querySelector('#selectedProducts tbody').appendChild(newRow);

//        // Update row indices
//        updateRowIndices();
//    }

//    // Update the total amount for the "Pay" button
//    updatePayButton();
//}
function handleProductAddition(encodedProductJson, quantityFromScanner, isUpdate = false) {
    debugger;
    var product = JSON.parse(encodedProductJson);
    var rowIndex = document.querySelectorAll('#selectedProducts tbody tr').length; // Zero-based index
    var suggestedNewProductId = document.getElementById('suggestedNewProductId').value;
    //var aa=$('#quantityAddpopup').data('productName');
    var existingRow = findProductRow(product.Name); // Find if product already exists in the table

    if (existingRow && isUpdate) {
        // Existing product logic, used for updating quantity
        var qtyInput = existingRow.cells[3].querySelector('input'); // Select input from qtyCell
        var totalCell = existingRow.cells[5]; // Assuming total price is the 6th cell (index 5)
        var currentQty = parseInt(qtyInput.value, 10);

        // Set the new quantity based on update flag
        var newQty = quantityFromScanner;

        // Calculate the new total price
        var newTotalPrice = newQty * product.SalePrice;
        totalCell.innerText = formatNumberWithDots(newTotalPrice);

        // Update quantity input
        qtyInput.value = newQty;

        // Update hidden inputs
        var hiddenQtyInput = existingRow.querySelector("[name^='SaleOrderDetail'][name$='Quantity']");
        hiddenQtyInput.value = newQty;

        // Update the totalVndBalanceHeader by adding the difference in price
        var oldTotalPrice = currentQty * product.SalePrice;
        totalVndBalanceHeader += newTotalPrice - oldTotalPrice;
        saveSelectedProductsToLocalStorage();
        isPupdateQuantity = false;

    } else if (existingRow && !isUpdate) {
        // If we are adding and the product already exists, just increase the quantity
        var qtyInput = existingRow.cells[3].querySelector('input'); // Select input from qtyCell
        var totalCell = existingRow.cells[5]; // Assuming total price is the 6th cell (index 5)
        var currentQty = parseInt(qtyInput.value, 10);

        // Increment by the new quantity
        var newQty = currentQty + quantityFromScanner;

        // Calculate the new total price
        var newTotalPrice = newQty * product.SalePrice;
        totalCell.innerText = formatNumberWithDots(newTotalPrice);

        // Update quantity input
        qtyInput.value = newQty;

        // Update hidden inputs
        var hiddenQtyInput = existingRow.querySelector("[name^='SaleOrderDetail'][name$='Quantity']");
        hiddenQtyInput.value = newQty;

        // Update the totalVndBalanceHeader by adding the difference in price
        var oldTotalPrice = currentQty * product.SalePrice;
        totalVndBalanceHeader += newTotalPrice - oldTotalPrice;
        saveSelectedProductsToLocalStorage();
    } else {
        // Create a new row if the product does not exist
        var newRow = document.createElement('tr');
        var indexCell = document.createElement('td');
        var nameCell = document.createElement('td');
        var priceCell = document.createElement('td');
        var qtyCell = document.createElement('td');
        var billNumberCell = document.createElement('td');
        var totalCell = document.createElement('td');
        totalCell.name = 'selectedProductTotal';
        totalCell.id = 'selectedProductTotal' + rowIndex;
        var actionsCell = document.createElement('td');

        // Fill the cells with product data
        indexCell.innerText = rowIndex + 1; // Row index starts from 1
        nameCell.innerText = product.Name;

        priceCell.innerText = formatNumberWithDots(product.SalePrice);
        priceCell.style.display = 'none'; // Hide Price cell

        // Create an editable input for quantity
        var qtyInput = document.createElement('input');
        qtyInput.type = 'number';
        qtyInput.min = '1';
        qtyInput.value = quantityFromScanner;
        qtyInput.className = 'form-control';
        qtyInput.style.width = '60px'; // Adjust width as needed
        // Listen for changes in the quantity input (on key press)
        qtyInput.addEventListener('input', function () {
            var newQty = parseInt(qtyInput.value, 10);
            if (isNaN(newQty) || newQty < 1) {
                newQty = 1; // Prevent invalid or negative values
                qtyInput.value = 1;
            }

            // Calculate the new total price
            var newTotalPrice = newQty * product.SalePrice;
            totalCell.innerText = formatNumberWithDots(newTotalPrice);

            // Update hidden quantity input
            hiddenQuantityInput.value = newQty;
            //localStorage.setItem('selectedProducts', JSON.stringify(product));
            // Recalculate total balance
            updatePayButton();
        });

        qtyInput.addEventListener('click', function () {
            debugger;
            $('#quantityAddpopup').modal('show');
            //$('#quantityAddpopup').on('shown.bs.modal', function () {
            //    //$('#productQuantity').focus().select();
            //    $('#productQuantity').focus();
            //});
            //// Highlight text and reset flag when input gains focus
            //$('#productQuantity').on('focus', function () {
            //    isFirstEntry = true; // Reset flag on focus
            //    focusedInput = $(this); // Set focusedInput to the current input
            //    $(this).select(); // Highlight the text in the input
            //});
            isPupdateQuantity = true;
            //$('#quantityAddpopup').data('productDetails', JSON.stringify(product));
        });

        // Append the input to the qtyCell
        qtyCell.appendChild(qtyInput);

        billNumberCell.innerText = suggestedNewProductId;
        billNumberCell.style.display = 'none'; // Hide bill number cell

        var totalAmount = quantityFromScanner * product.SalePrice;
        totalCell.innerText = formatNumberWithDots(totalAmount);

        totalVndBalanceHeader += totalAmount;

        // Create hidden inputs
        var hiddenProductIdInput = document.createElement('input');
        hiddenProductIdInput.type = 'hidden';
        hiddenProductIdInput.name = 'SaleOrderDetail[' + rowIndex + '].ProductId';
        hiddenProductIdInput.value = product.Id;

        var hiddenProductNameInput = document.createElement('input');
        hiddenProductNameInput.type = 'hidden';
        hiddenProductNameInput.name = 'SaleOrderDetail[' + rowIndex + '].Product.Name';
        hiddenProductNameInput.value = product.Name;

        var hiddenSalePriceInput = document.createElement('input');
        hiddenSalePriceInput.type = 'hidden';
        hiddenSalePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].SalePrice';
        hiddenSalePriceInput.value = product.SalePrice;

        var hiddenPurchasePriceInput = document.createElement('input');
        hiddenPurchasePriceInput.type = 'hidden';
        hiddenPurchasePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].PurchasePrice';
        hiddenPurchasePriceInput.value = product.PurchasePrice;

        var hiddenQuantityInput = document.createElement('input');
        hiddenQuantityInput.type = 'hidden';
        hiddenQuantityInput.name = 'SaleOrderDetail[' + rowIndex + '].Quantity';
        hiddenQuantityInput.value = quantityFromScanner;

        // Add action button for row deletion
        var removeButton = document.createElement('button');
        removeButton.type = 'button';
        removeButton.className = 'btn btn-sm';
        var icon = document.createElement('i');
        icon.className = 'material-icons';
        icon.innerText = 'delete';
        icon.style.color = '#FF6F6F';
        removeButton.appendChild(icon);
        removeButton.onclick = function () {
            removeProductsEntireRow(newRow, product.SalePrice);
        };
        actionsCell.appendChild(removeButton);

        // Append the cells to the new row
        newRow.appendChild(indexCell);
        newRow.appendChild(nameCell);
        newRow.appendChild(priceCell);
        newRow.appendChild(qtyCell);
        newRow.appendChild(billNumberCell);
        newRow.appendChild(totalCell);
        newRow.appendChild(actionsCell);

        // Append hidden inputs to the row
        newRow.appendChild(hiddenProductIdInput);
        newRow.appendChild(hiddenProductNameInput);
        newRow.appendChild(hiddenSalePriceInput);
        newRow.appendChild(hiddenPurchasePriceInput);
        newRow.appendChild(hiddenQuantityInput);

        // Append the new row to the table body
        document.querySelector('#selectedProducts tbody').appendChild(newRow);

        // Update row indices
        updateRowIndices();

        // After adding the row, store all selected products in localStorage
        saveSelectedProductsToLocalStorage();
    }

    // Update the total amount for the "Pay" button
    updatePayButton();
}
// Utility function to set a cookie
function setCookie(name, value, days) {
    debugger;
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000)); // Convert days to milliseconds
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

// Utility function to get a cookie by name
function getCookie(name) {
    const nameEQ = name + "=";
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
// Function to remove a cookie by setting its expiration to the past
function deleteCookie(name) {
    setCookie(name, "", -1);  // Setting the expiration date to the past
}
function saveSelectedProductsToLocalStorage() {
    debugger;
    const rows = document.querySelectorAll('#selectedProducts tbody tr');
    const allProducts = [];

    // Loop through each row
    rows.forEach((row, index) => {
        // Get hidden input values for each product
        const productId = row.querySelector(`input[name='SaleOrderDetail[${index}].ProductId']`).value;
        const productName = row.querySelector(`input[name='SaleOrderDetail[${index}].Product.Name']`).value;
        const salePrice = row.querySelector(`input[name='SaleOrderDetail[${index}].SalePrice']`).value;
        const purchasePrice = row.querySelector(`input[name='SaleOrderDetail[${index}].PurchasePrice']`).value;
        const quantity = row.querySelector(`input[name='SaleOrderDetail[${index}].Quantity']`).value;

        // Push product details into the allProducts array
        allProducts.push({
            productId,
            productName,
            salePrice,
            purchasePrice,
            quantity
        });
    });

    // Log all products to the console
    //console.log(allProducts);
    // Store the array of selected products in localStorage as a JSON string
    // Return the array of all products
    //localStorage.setItem('selectedProducts', JSON.stringify(allProducts));
    const productsJSON = JSON.stringify(allProducts);
    //setCookie('selectedProducts', productsJSON, 1);  // 1 day expiration commented just for testing
    const connection = $.connection.phevaHub;
    //connection.client.broadcastProductUpdate = function (productsJSON) {
    //    debugger;
    //    console.log('Received product update in Detail view:', productsJSON);
    //    ////var products = JSON.parse(productsJSON);
    //    //debugger;
    //    //$('#productDetailsTable tbody').empty();

    //    //// Populate the table with new data
    //    //productsJSON.forEach(function (product) {
    //    //    $('#productDetailsTable tbody').append(
    //    //        `<tr>
    //    //            <td>${product.ProductId}</td>
    //    //            <td>${product.ProductName}</td>
    //    //            <td>${product.SalePrice}</td>
    //    //            <td>${product.PurchasePrice}</td>
    //    //            <td>${product.Quantity}</td>
    //    //        </tr>`
    //    //    );
    //    //});
    //    // Call the function to update the table with the received data
    //    //updateProductTable(productsJSON);
    //};
    //$.connection.hub.start({ transport: 'longPolling' })
    //$.connection.hub.start({ transport: ['webSockets', 'longPolling', 'serverSentEvents'] })
    //////Payment Details
    $.connection.hub.start({ transport: ['webSockets','longPolling', , 'serverSentEvents'] })
        .done(function () {
            console.log('Connection started!');
            connection.server.broadcastProductUpdate(productsJSON);
        })
        .fail(function (error) {
            console.error('Could not connect to SignalR hub:', error);
        });
    //window.location.href = 'ProductDetailsCustomer'; 
    return allProducts;
}
// Function to update the product details table
//function updateProductTable(products) {
//    var tableBody = $('#productDetailsTable tbody');
//    tableBody.empty(); // Clear existing rows

//    // Check if products is an array
//    if (Array.isArray(products) && products.length > 0) {
//        products.forEach(function (product) {
//            $('#testInput').val(product.ProductName);
//            // Create a new row for each product
//            var row = `<tr>
//                <td>${product.ProductId}</td>
//                <td>${product.ProductName}</td>
//                <td>${product.SalePrice}</td>
//                <td>${product.PurchasePrice}</td>
//                <td>${product.Quantity}</td>
//            </tr>`;
//            tableBody.append(row); // Append the new row to the table body
//        });
//    } else {
//        // If no products are available, show a message
//        tableBody.append('<tr><td colspan="5">No products available.</td></tr>');
//    }
//}

//function handleProductAddition(encodedProductJson, quantityFromScanner) {
//    var product = JSON.parse(encodedProductJson);
//    var rowIndex = document.querySelectorAll('#selectedProducts tbody tr').length; // Zero-based index
//    var suggestedNewProductId = document.getElementById('suggestedNewProductId').value;
//    var existingRow = findProductRow(product.Name);

//    if (existingRow) {
//        // Existing product logic
//        var qtyInput = existingRow.cells[3].querySelector('input'); // Select input from qtyCell
//        var totalCell = existingRow.cells[5]; // Assuming total price is the 6th cell (index 5)
//        var currentQty = parseInt(qtyInput.value, 10);
//        var newQty = currentQty + quantityFromScanner;

//        // Calculate the new total price
//        var newTotalPrice = newQty * product.SalePrice;
//        totalCell.innerText = formatNumberWithDots(newTotalPrice);

//        // Update quantity input
//        qtyInput.value = newQty;

//        // Update hidden inputs
//        var hiddenQtyInput = existingRow.querySelector("[name^='SaleOrderDetail'][name$='Quantity']");
//        hiddenQtyInput.value = newQty;

//        // Update the totalVndBalanceHeader by adding the difference in price
//        var oldTotalPrice = currentQty * product.SalePrice;
//        totalVndBalanceHeader += newTotalPrice - oldTotalPrice;
//    } else {
//        // Create a new row if the product does not exist
//        var newRow = document.createElement('tr');
//        var indexCell = document.createElement('td');
//        var nameCell = document.createElement('td');
//        var priceCell = document.createElement('td');
//        var qtyCell = document.createElement('td');
//        var billNumberCell = document.createElement('td');
//        var totalCell = document.createElement('td');
//        totalCell.name = 'selectedProductTotal';
//        totalCell.id = 'selectedProductTotal' + rowIndex;
//        var actionsCell = document.createElement('td');

//        // Fill the cells with product data
//        indexCell.innerText = rowIndex + 1; // Row index starts from 1
//        nameCell.innerText = product.Name;

//        priceCell.innerText = formatNumberWithDots(product.SalePrice);
//        priceCell.style.display = 'none'; // Hide Price cell

//        // Create an editable input for quantity
//        var qtyInput = document.createElement('input');
//        qtyInput.type = 'number';
//        qtyInput.min = '1';
//        qtyInput.value = quantityFromScanner;
//        qtyInput.className = 'form-control';
//        qtyInput.style.width = '60px'; // Adjust width as needed

//        // Listen for changes in the quantity input
//        qtyInput.onchange = function () {
//            var newQty = parseInt(qtyInput.value, 10);
//            var newTotalPrice = newQty * product.SalePrice;
//            totalCell.innerText = formatNumberWithDots(newTotalPrice);

//            // Update hidden quantity input
//            hiddenQuantityInput.value = newQty;

//            // Update totalVndBalanceHeader by recalculating the total price difference
//            updateTotalVndBalance();
//        };

//        // Append the input to the qtyCell
//        qtyCell.appendChild(qtyInput);

//        billNumberCell.innerText = suggestedNewProductId;
//        billNumberCell.style.display = 'none'; // Hide bill number cell

//        var totalAmount = quantityFromScanner * product.SalePrice;
//        totalCell.innerText = formatNumberWithDots(totalAmount);

//        totalVndBalanceHeader += totalAmount;

//        // Create hidden inputs
//        var hiddenProductIdInput = document.createElement('input');
//        hiddenProductIdInput.type = 'hidden';
//        hiddenProductIdInput.name = 'SaleOrderDetail[' + rowIndex + '].ProductId';
//        hiddenProductIdInput.value = product.Id;

//        var hiddenProductNameInput = document.createElement('input');
//        hiddenProductNameInput.type = 'hidden';
//        hiddenProductNameInput.name = 'SaleOrderDetail[' + rowIndex + '].Product.Name';
//        hiddenProductNameInput.value = product.Name;

//        var hiddenSalePriceInput = document.createElement('input');
//        hiddenSalePriceInput.type = 'hidden';
//        hiddenSalePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].SalePrice';
//        hiddenSalePriceInput.value = product.SalePrice;

//        var hiddenPurchasePriceInput = document.createElement('input');
//        hiddenPurchasePriceInput.type = 'hidden';
//        hiddenPurchasePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].PurchasePrice';
//        hiddenPurchasePriceInput.value = product.PurchasePrice;

//        var hiddenQuantityInput = document.createElement('input');
//        hiddenQuantityInput.type = 'hidden';
//        hiddenQuantityInput.name = 'SaleOrderDetail[' + rowIndex + '].Quantity';
//        hiddenQuantityInput.value = quantityFromScanner;

//        // Add action button for row deletion
//        var removeButton = document.createElement('button');
//        removeButton.type = 'button';
//        removeButton.className = 'btn btn-sm';
//        var icon = document.createElement('i');
//        icon.className = 'material-icons';
//        icon.innerText = 'delete';
//        icon.style.color = '#FF6F6F';
//        removeButton.appendChild(icon);
//        removeButton.onclick = function () {
//            removeProductsEntireRow(newRow, product.SalePrice);
//        };
//        actionsCell.appendChild(removeButton);

//        // Append the cells to the new row
//        newRow.appendChild(indexCell);
//        newRow.appendChild(nameCell);
//        newRow.appendChild(priceCell);
//        newRow.appendChild(qtyCell);
//        newRow.appendChild(billNumberCell);
//        newRow.appendChild(totalCell);
//        newRow.appendChild(actionsCell);

//        // Append hidden inputs to the row
//        newRow.appendChild(hiddenProductIdInput);
//        newRow.appendChild(hiddenProductNameInput);
//        newRow.appendChild(hiddenSalePriceInput);
//        newRow.appendChild(hiddenPurchasePriceInput);
//        newRow.appendChild(hiddenQuantityInput);

//        // Append the new row to the table body
//        document.querySelector('#selectedProducts tbody').appendChild(newRow);

//        // Update row indices
//        updateRowIndices();
//    }

//    // Update the total amount for the "Pay" button
//    updatePayButton();
//}

////Working Code
//function handleProductAddition(encodedProductJson, quantityFromScanner) {
//    //debugger;
//    var product = JSON.parse(encodedProductJson);
//    var rowIndex = document.querySelectorAll('#selectedProducts tbody tr').length; // Zero-based index

//    //    // Retrieve the SuggestedNewProductId value from the hidden input field
//    var suggestedNewProductId = document.getElementById('suggestedNewProductId').value;

//    //    // Search for the product in the table
//    var existingRow = findProductRow(product.Name);

//    if (existingRow) {
//        // Existing product logic
//        var qtyCell = existingRow.cells[3]; // Assuming quantity is the 4th cell (index 3)
//        var totalCell = existingRow.cells[5]; // Assuming total price is the 6th cell (index 5)
//        var currentQty = parseInt(qtyCell.innerText, 10);
//        var newQty = currentQty + quantityFromScanner;

//        // Calculate the new total price
//        var newTotalPrice = newQty * product.SalePrice;

//        // Format the total price using the custom function
//        totalCell.innerText = formatNumberWithDots(newTotalPrice);

//        // Update quantity
//        qtyCell.innerText = newQty;

//        // Update hidden inputs
//        var hiddenQtyInput = existingRow.querySelector("[name^='SaleOrderDetail'][name$='Quantity']");
//        hiddenQtyInput.value = newQty;

//        // Update the totalVndBalanceHeader by adding the difference in price
//        var oldTotalPrice = currentQty * product.SalePrice;
//        totalVndBalanceHeader += newTotalPrice - oldTotalPrice;
//    } else {
//        // Create a new row if the product does not exist
//        var newRow = document.createElement('tr');
//        var indexCell = document.createElement('td');
//        var nameCell = document.createElement('td');
//        var priceCell = document.createElement('td');
//        var qtyCell = document.createElement('td');
//        var billNumberCell = document.createElement('td'); // Bill Number Cell
//        var totalCell = document.createElement('td');
//        totalCell.name = 'selectedProductTotal';
//        totalCell.id = 'selectedProductTotal' + rowIndex; // Make sure ID is unique per row
//        var actionsCell = document.createElement('td');

//        // Fill the cells with product data
//        indexCell.innerText = rowIndex + 1; // Row index starts from 1
//        nameCell.innerText = product.Name;

//        // Format the SalePrice using the custom function
//        priceCell.innerText = formatNumberWithDots(product.SalePrice);

//        // Hide Price cell
//        priceCell.style.display = 'none';

//        qtyCell.innerText = quantityFromScanner;

//        // Directly set Bill Number to SuggestedNewProductId (do not perform calculations on it)
//        billNumberCell.innerText = suggestedNewProductId;
//        // Hide bill NumberCell cell
//        billNumberCell.style.display = 'none';

//        // Calculate total based on SalePrice and quantity
//        var totalAmount = quantityFromScanner * product.SalePrice;
//        totalCell.innerText = formatNumberWithDots(totalAmount);

//        // Accumulate the total price to totalVndBalanceHeader
//        totalVndBalanceHeader += totalAmount;

//        // Create hidden inputs
//        var hiddenProductIdInput = document.createElement('input');
//        hiddenProductIdInput.type = 'hidden';
//        hiddenProductIdInput.name = 'SaleOrderDetail[' + rowIndex + '].ProductId';
//        hiddenProductIdInput.value = product.Id;

//        var hiddenProductNameInput = document.createElement('input');
//        hiddenProductNameInput.type = 'hidden';
//        hiddenProductNameInput.name = 'SaleOrderDetail[' + rowIndex + '].Product.Name';
//        hiddenProductNameInput.value = product.Name;

//        var hiddenSalePriceInput = document.createElement('input');
//        hiddenSalePriceInput.type = 'hidden';
//        hiddenSalePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].SalePrice';
//        hiddenSalePriceInput.value = product.SalePrice;

//        var hiddenPurchasePriceInput = document.createElement('input');
//        hiddenPurchasePriceInput.type = 'hidden';
//        hiddenPurchasePriceInput.name = 'SaleOrderDetail[' + rowIndex + '].PurchasePrice';
//        hiddenPurchasePriceInput.value = product.PurchasePrice;

//        var hiddenQuantityInput = document.createElement('input');
//        hiddenQuantityInput.type = 'hidden';
//        hiddenQuantityInput.name = 'SaleOrderDetail[' + rowIndex + '].Quantity';
//        hiddenQuantityInput.value = quantityFromScanner;

//        // Add an action button
//        var removeButton = document.createElement('button');
//        removeButton.type = 'button';
//        removeButton.className = 'btn btn-sm';
//        var icon = document.createElement('i');
//        icon.className = 'material-icons';
//        icon.innerText = 'delete';
//        icon.style.color = '#FF6F6F';
//        removeButton.appendChild(icon);
//        removeButton.onclick = function () {
//            //removeProduct(newRow, product.SalePrice); single product deletion
//            removeProductsEntireRow(newRow, product.SalePrice);
//        };
//        actionsCell.appendChild(removeButton);

//        // Append the cells to the new row
//        newRow.appendChild(indexCell);
//        newRow.appendChild(nameCell);
//        newRow.appendChild(priceCell);
//        newRow.appendChild(qtyCell);
//        newRow.appendChild(billNumberCell); // Append Bill Number Cell
//        newRow.appendChild(totalCell);
//        newRow.appendChild(actionsCell);

//        // Append hidden inputs to the row
//        newRow.appendChild(hiddenProductIdInput);
//        newRow.appendChild(hiddenProductNameInput);
//        newRow.appendChild(hiddenSalePriceInput);
//        newRow.appendChild(hiddenPurchasePriceInput);
//        newRow.appendChild(hiddenQuantityInput);

//        // Append the new row to the table body
//        document.querySelector('#selectedProducts tbody').appendChild(newRow);

//        // Update row indices
//        updateRowIndices();
//    }

//    // Update the total amount for the "Pay" button
//    updatePayButton();
//}
// Function to find an existing product row by product name
function findProductRow(productName) {
    //debugger;
    var rows = document.querySelectorAll('#selectedProducts tbody tr');
    for (var i = 0; i < rows.length; i++) {
        // Make sure to compare with the exact product name
        if (rows[i].cells[1].innerText.trim() === productName.trim()) { // Assuming product name is in the 2nd cell (index 1)
            return rows[i];
        }
    }
    return null;
}

// Function to remove a product row and update the quantities and total working
//function removeProduct(row, pricePerUnit) {
//    var qtyCell = row.cells[3]; // Assuming quantity is the 4th cell (index 3)
//    var totalCell = row.cells[5]; // Assuming total price is the 5th cell (index 4)
//    var currentQty = parseInt(qtyCell.innerText, 10);

//    if (currentQty > 1) {
//        // Decrease the quantity if more than 1
//        var newQty = currentQty - 1;
//        qtyCell.innerText = newQty;
//        //totalCell.innerText = (newQty * pricePerUnit).toFixed(2); // Update total price
//        totalCell.innerText = formatNumberWithDots((newQty * pricePerUnit)); // Update total price
//    } else {
//        // Remove the row if quantity is 1
//        row.remove();
//        updateRowIndices(); // Update indices after a row is removed
//    }

//    // Update the total amount for the "Pay" button
//    updatePayButton();
//}



//Function to remove Entire row
function removeProductsEntireRow(row, pricePerUnit) {
    // Regardless of quantity, remove the entire row
    row.remove();

    // Update the row indices after the row is removed (if necessary)
    updateRowIndices();

    // Update the total amount for the "Pay" button
    updatePayButton();
    saveSelectedProductsToLocalStorage();
    //setCookie('selectedProducts', productsJSON, 1);
    //deleteCookie('selectedProducts');
    //localStorage.removeItem('selectedProducts');
}

// Function to update the total amount for the "Pay" button
var totalAmountForSelectedProduct = 0;
function updatePayButton() {
    //debugger;
    //var total = 0;
    //document.querySelectorAll('#selectedProducts tbody tr').forEach(function (row) {
    //    var totalCellText = row.cells[4].innerText.trim(); // Get the total cell text and trim extra spaces
    //    var cellValue = parseFloat(totalCellText); // Parse the text to a float

    //    if (isNaN(cellValue)) {
    //        console.error("Invalid value in total cell: " + totalCellText);
    //    } else {
    //        total += cellValue;
    //    }
    //});

    //// Update the "Pay" button text with the total amount
    //$("#CreateSO").html("Pay " + total.toFixed(2));
    //var totalAmount = 0;

    //// Iterate through each row in the table to calculate the total amount
    //$('#selectedProducts tbody tr').each(function () {
    //    var totalCell = $(this).find('td:nth-child(5)').text(); // Assuming the total price is in the 5th cell
    //    totalAmount += parseFloat(totalCell) || 0; // Add to totalAmount, defaulting to 0 if NaN
    //});

    //// Update the "Total Amount" display element with the calculated total amount
    ////$('#ItemsTotal').val(); // Assuming '#ItemsTotal' is the total field ID

    //totalPayableBill = totalAmount;
    //totalBillPaid = totalAmount;
    //debugger;
    var totalAmount = 0;

    // Iterate through each row in the table body to calculate the total amount
    $('#selectedProducts tbody tr').each(function () {
        // Find the text of the total cell, which is in the 5th cell (index 5 in your updated code)
        var totalCell = $(this).find('td:nth-child(6)').text().trim(); // Ensure no spaces around the number

        // Remove dots from the number to prepare for conversion
        var totalCellNumber = parseFloat(totalCell.replace(/\./g, '')) || 0; // Replace all dots and parse as float

        // Add the parsed number to the totalAmount
        totalAmount += totalCellNumber;
    });

    // Update the "Total Amount" display element with the calculated total amount
    $('#ItemsTotal').val(totalAmount.toLocaleString('de-DE')); // Format number with dots again if needed

    // Set totalPayableBill and totalBillPaid to the calculated total amount
    totalPayableBill = totalAmount;
    totalBillPaid = totalAmount;

    // Optionally update the Pay button text
    $("#CreateSO")
        .html("Pay")                // Set the visible text to "Pay"
        .attr("data-amount", totalAmount);

    $("#totalofAllSelecteProducts").attr("data-amounts", totalAmount);

    // Update the total of all selected products
    var getSelectedProductTotal = totalAmount.toLocaleString('de-DE');
    $("#totalofAllSelecteProducts").html("₫ " + getSelectedProductTotal); // Update with the formatted total amount
    totalAmountForSelectedProduct = getSelectedProductTotal;
    totalVndBalanceHeader = getSelectedProductTotal;
    //var totalAmount = 0;

    //// Iterate through each row in the table body
    //$('#selectedProducts tbody tr').each(function () {
    //    // Find the text of the total cell, which is in the 5th cell (index 4)
    //    var totalCell = $(this).find('td:nth-child(6)').text().trim(); // Ensure no spaces around the number

    //    // Remove dots from the number to prepare for conversion
    //    var totalCellNumber = parseFloat(totalCell.replace(/\./g, '')) || 0; // Replace all dots and parse as float

    //    // Add the parsed number to the totalAmount
    //    totalAmount += totalCellNumber;
    //});

    //// Update the "Total Amount" display element with the calculated total amount
    //// Assuming '#ItemsTotal' is the total field ID
    //$('#ItemsTotal').val(totalAmount.toLocaleString('de-DE')); // Format number with dots again if needed

    //// Set totalPayableBill and totalBillPaid to the calculated total amount
    //totalPayableBill = totalAmount;
    //totalBillPaid = totalAmount;

    ////totalPayableBill = totalAmount.toFixed(2);
    ////totalBillPaid = totalAmount.toFixed(2);

    //// Optionally update the Pay button text
    //// $("#CreateSO").html("Pay " + totalAmount.toFixed(2)); // Assuming '#PayButton' is the button ID
    //$("#CreateSO")
    //    .html("Pay")                // Set the visible text to "Pay"
    //    .attr("data-amount", totalAmount);
    ////.attr("data-amount", totalAmount.toFixed(2));
    ////$("#totalofAllSelecteProducts").html("₫ "+ totalAmount.toFixed(2)); // Assuming '#PayButton' is the button ID

    //var getSelectedProductTotal = document.getElementById('selectedProductTotal').textContent;
    ////alert(getSelectedProductTotal);
    ////$("#totalofAllSelecteProducts").html("₫ " + formatNumberWithDots(totalAmount)); // Assuming '#PayButton' is the button ID
    //$("#totalofAllSelecteProducts").html("₫ " + getSelectedProductTotal); // Assuming '#PayButton' is the button ID
}

// Function to update row indices after a row is added or removed
function updateRowIndices() {
    document.querySelectorAll('#selectedProducts tbody tr').forEach(function (row, index) {
        row.cells[0].innerText = index + 1; // Assuming the index cell is the first cell (index 0)
    });
}
//function updateSalesForm(salePrice) {
//    debugger;
//    var aa = $('#ItemsTotal').val(parseFloat(salePrice.value).toFixed(2));
//    $('#ItemsTotal').val(parseFloat(salePrice.value).toFixed(2));
//}
$(function () {
    //OnTypeName('#name0');
    //alert('#' + clickedTextboxId);


    ConfigDialogueCreateCustomer();

});

var _keybuffer = "";

$(document).ready(function () {
    //alert(products);
    if (IsReturn == 'true') {
        $('#saleOrder').hide();
        $('#saleReturn').show();
        $("#pd3").val("Returned");
    }
    else {
        $('#saleReturn').hide();
        $('#saleOrder').show();
        $("#pd3").val("Paid");
    }

    $('#abc').scannerDetection({

        //https://github.com/kabachello/jQuery-Scanner-Detection

        timeBeforeScanTest: 200, // wait for the next character for upto 200ms
        avgTimeByChar: 40, // it's not a barcode if a character takes longer than 100ms
        //preventDefault: false,

        endChar: [13],
        onComplete: function (barcode, qty) {
            validScan = true;
            //$('#customer').val(barcode);
            //alert(barcode);


            var activeControlId = $(document.activeElement).attr("id");
            //alert(activeControlId);

            if (activeControlId.substring(0, 4) != 'name') {
                alert("To scan product barcode, please place cursor in product name text box.");
                return false;
            }

            //var result = $.grep(products, function (e) { return e.Barcode == barcode; });
            //alert('lakdsfjkljs');

            var result = [];
            //if (typeof result === "undefined") {
            //    alert("something is undefined");
            //}

            //for (var i = 0, len = products.length; i < len; i++) {
            //    alert(products[i]);
            //    if (products[i][0] === barcode) {
            //        result = products[i];
            //        break;
            //    }
            //}

            //var abc = productsBarcodes[2];
            //alert(productsBarcodes.length);
            for (var i = 0, len = productsBarcodes.length; i < len; i++) {
                //alert(productsBarcodes[i][1]);
                if (productsBarcodes[i][1] === barcode) {
                    /*alert('found');*/
                    result = products[i];
                    break;
                }
            }


            if (result.length === 0) {
                alert('unfortunately, No item found against this barcode ');
                return false;
            }


            pfound = 0;
            $('#selectedProducts > tbody  > tr').each(function () {

                if ($(this).find("[id^='idn']").val() == result[0]) {
                    num = + $(this).find("[id^='quantity']").val() + 1;
                    $(this).find("[id^='quantity']").val(num);
                    //alert($(this).find("[id^='quantity']").val());
                    //$(this).find("[id^='quantity']").val() += 1;
                    //alert(ui.item[0]);
                    update_itemTotal();
                    pfound = 1;
                    return false;
                }
            })
            if (pfound == 0) {
                $('#name' + clickedIdNum).val(ui.item ? ui.item[1] : '');
                $('#perPack' + clickedIdNum).val(ui.item ? ui.item[4] : '');
                //}

                $('#salePrice' + clickedIdNum).val(ui.item ? ui.item[2] : '');
                $('#quantity' + clickedIdNum).val(ui.item ? 1 : '');
                $('#idn' + clickedIdNum).val(ui.item ? ui.item[0] : '');
                update_itemTotal();
                $('#addNewRow').trigger('click');
            }






        } // main callback function	,
        //,
        //onError: function (string, qty) {
        //    var activeControlId = $(document.activeElement).attr("id");
        //    if (activeControlId.substring(0, 4) != 'name') {
        //        alert("To scan product barcode, please place cursor in product name text box.");
        //        return false;
        //    }
        //    alert("unfortunately not found product against this barcode" );
        //    //$('#customerAddress').val($('#customerAddress').val() + string);
        //}


    });

    //$(this).closest('form').find("input[type=text], textarea").val("");

    //alert('iam ready');
    document.getElementById('vndCustomer').focus();

    //$('#name').tooltip('show')

    //$('[data-toggle="tooltip"]').tooltip();
    var actions = $("table td:last-child").html();
    // Append table with add row form on add new button click

    $('#addNewRow').keydown(function (event) {
        //alert('keydown');
        if (event.keyCode == 13) {
            $('#addNewRow').trigger('click');
        }
    });

    $("#addNewRow").click(function (e) {
        //alert('click');
        //var key = e.which;
        //if (key !== 13)  // the enter key code
        //{

        //    return false;
        //}

        //$(this).attr("disabled", "disabled");
        txtSerialNum += 1;
        //alert(txtSerialNum)
        var index = $("table tbody tr:last-child").index();
        //var rowCount = 
        var row = '<tr>' +
            '<td id="SNo' + txtSerialNum + '">' + $('#selectedProducts tr').length + '</td>' +
            '<td style="display:none;"><input type="hidden" name="SaleOrderDetail.Index" value="' + txtSerialNum + '" /></td>' +
            '<td style="display:none;"><input type="text" readonly class="form-control classBGcolor" name="SaleOrderDetail[' + txtSerialNum + '].ProductId" id="idn' + txtSerialNum + '"></td>' +
            '<td><input type="text" class="form-control" autocomplete="off" name="SaleOrderDetail[' + txtSerialNum + '].Product.Name" id="name' + txtSerialNum + '"></td>' +
            '<td><input type="text"  class="form-control autocomplete="off" classBGcolor" name="SaleOrderDetail[' + txtSerialNum + '].SalePrice" id="salePrice' + txtSerialNum + '"></td>' +
            '<td><input type="text" class="form-control" autocomplete="off" name="SaleOrderDetail[' + txtSerialNum + '].Quantity" id="quantity' + txtSerialNum + '"></td>' +
            '<td style="display:none;"><select class="form-control" name="SaleOrderDetail[' + txtSerialNum + '].IsPack" id="isPack' + txtSerialNum + '"><option value="false">Piece</option><option value="true" selected>Pack</option></select></td>' +
            '<td style="display:none;"><input type="text" class="form-control" readonly autocomplete="off" name="SaleOrderDetail[' + txtSerialNum + '].PerPack" id="perPack' + txtSerialNum + '"></td>' +

            '<td><input type="text" readonly class="form-control classBGcolor" name="itemTotal' + txtSerialNum + '" id="itemTotal' + txtSerialNum + '"tabindex="-1"></td>' +
            '<td style="display:none;"><select class="form-control" name="SaleOrderDetail[' + txtSerialNum + '].SaleType" id="saleType' + txtSerialNum + '"><option value="false" selected>Order</option><option value="true">Return</option></select></td>' +
            '<td><button type="button" id="delete' + txtSerialNum + '" class="delete btn btn-default add-new"> <a class="delete" title="Delete" data-toggle="tooltip"><i class="material-icons">&#xE872;</i></a></button></td>' +
            '</tr>';

        //alert(row);
        $("#selectedProducts").append(row);
        //alert(txtSerialNum)

        $("table tbody tr").eq(index + 1).find(".add, .edit").toggle();
        //$('[data-toggle="tooltip"]').tooltip();

        document.getElementById('name' + txtSerialNum).focus();
        TriggerBodyEvents();

    });


    // Edit row on edit button click
    $(document).on("click", ".edit", function () {
        $(this).parents("tr").find("td:not(:last-child)").each(function () {
            $(this).html('<input type="text" class="form-control" value="' + $(this).text() + '">');
        });
        $(this).parents("tr").find(".add, .edit").toggle();
        $("#addNewRow").attr("disabled", "disabled");
    });



    //$(document).on("keyup", "#delete1", function (event) {
    //    if (event.keyCode == 13) {
    //        $("#delete1").trigger('click');
    //    }
    //});



    // Delete row on delete button click
    //$(document).on("click", "#delete1", function () {
    //    $(this).parents("tr").remove();
    //    $("#addNewRow").removeAttr("disabled");
    //    update_itemTotal();
    //});
    $(document).on("keypress", "form", function (event) {
        return event.keyCode != 13;
    });
    //$('td[id^="' + value +'"]')         "[id^='quantity']"


    TriggerFooterEvents();

    //$("[id^='quantity']").keydown(function (e) {
    //    // Allow: backspace, delete, tab, escape, enter and .(45) and -(46)
    //    if ($.inArray(e.keyCode, [8, 9, 27, 13, 110]) !== -1 ||

    //        // Allow: Ctrl+A, Command+A
    //        (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
    //        // Allow: home, end, left, right, down, up
    //        (e.keyCode >= 35 && e.keyCode <= 40)) {
    //        // let it happen, don't do anything

    //        return;
    //    }
    //    // Ensure that it is a number and stop the keypress
    //    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
    //        e.preventDefault();
    //    }
    //});
    $('#deleteAllProducts').click(function (e) {
        // Prevent default action if necessary
        e.preventDefault();
        // Alert to show the function is being triggered
        //alert('Hi');
        // Remove all rows from the tbody
        $('#selectedProducts tbody').empty();
        //$("#CreateSO").html("Pay " + '0.00');
        $("#CreateSO").html("Pay ");
        $("#totalofAllSelecteProducts").html("₫ " + '0.00');
        //$('#cashvnd').val('0');
        //$('#cardvnd').val('0');
        totalAmountForSelectedProduct = 0;
        totalVndBalanceHeader = 0;
        //$('#payallbycard').text('Pay all by card');
    });
    $('#CreateSO').keydown(function (event) {
        debugger;
        if (event.keyCode == 13) {
            $('#CreateSO').trigger('click');
        }
    });
    $('#saveSales').click(function (event) {
        debugger;
        const customerId = $('#customerId').val();
        const vndCustomer = $('#vndCustomer').val();
        const customerName = $('#vndCustomerName').val();
        const customerVat = $('#vndCustomerVat').val();
        const customerCompany = $('#vndCustomerCompany').val();
        const customerAddress = $('#vndCustomerAddress').val();
        const customerEmail = $('#vndCustomerEmail').val();

        // Set the hidden fields with the values
        $('#customerId1').val(customerId);
        $('#vndCustomer1').val(customerName);
        $('#vndCustomerCompany1').val(customerCompany);
        $('#vndCustomerName1').val(customerName);
        $('#vndCustomerVat1').val(customerVat);
        $('#vndCustomerAddress1').val(customerAddress);
        $('#vndCustomerEmail1').val(customerEmail);

        //$('input[name="Customer.Id"]').val(customerId);
        //$('input[name="Customer.Vat"]').val(customerVat);
        //$('input[name="Customer.Name"]').val(customerName);
        //$('input[name="Customer.CompanyName"]').val(customerCompany);
        //$('input[name="Customer.Address"]').val(customerAddress);
        //$('input[name="Customer.Email"]').val(customerEmail);
        // Log values to debug (optional)
        //console.log(customerId, customerName, customerVat);

        //Binding SaleOrder Form
        $('#cardvnd').val(cardVndBalance);
        $('#cashvnd').val(cashVndBalance);
        $('#ItemsTotal').val(totalPayableBill);
        $('#paid').val(cardVndBalance);
        $('#paidByCash').val(cashVndBalance);

        //const cardVnd = $('#cardvnd').val(cardVndBalance);
        //const cashvnd = $('#cashvnd').val(cashVndBalance);
        //const itemsTotal = $('#ItemsTotal').val(totalPayableBill);
        //const paid = $('#paid').val(cardVndBalance);
        //const paidByCash = $('#paidByCash').val(cashVndBalance);

        //$('#cardvnd1').val(cardVnd1);
        //$('#cashvnd1').val(cashvnd1);
        //$('#paidByCash1').val(paidByCash1);
        //$('#ItemsTotal').val(itemsTotal1);
        //$('#paid1').val(paid1);
        ///

        //$('#paid').val(totalBillPaid);


        if (leftVndBalance === null || leftVndBalance === undefined || leftVndBalance === "") {
            $('#balance').val('0.00');
            $('#balance1').val('0.00');
        } else {
            $('#balance').val(leftVndBalance);
            $('#balance1').val(leftVndBalance);
        }
        event.preventDefault(); // Prevent the default action (if any)
        //var totalpaymentText = $('#CreateSO').text();
        //var totalpayment = totalpaymentText.replace('Pay ', '').trim();
        /*$('#ItemsTotal').val(totalpayment);*/
        $('#SOCreate').submit();
    });

    $('#CreateSO').click(function () {
        debugger;
        $('#ItemsTotal').val(totalPayableBill);
        //$('#ItemsTotal1').val(totalPayableBill);
        //console.log('BillAmount:', $('#ItemsTotal').val());
        //console.log('Discount:', $('#discount').val());
        //if ($('#idnCustomer').val() == "") {
        //    alert('Customer not found. Please select customer from list or add new');
        //    return false;
        //}


        //-----------------------------------------------------------------v-product stock check-v--------------------------
        //var EnteredQty1 = 0;
        //var EnteredProductId1 = 0;
        //var tblProductStock1 = 0;
        //var tblProductId = 0;
        //var saleType1 = false;
        //var idx1 = 0;
        //var isFalse = true;
        //$('#selectedProducts tbody tr').each(function () {
        //    idx1 += 1;
        //    EnteredQty1 = $(this).closest("tr").find("[id^='quantity']").val();
        //    EnteredProductId1 = $(this).closest("tr").find("[id^='idn']").val().trim();
        //    saleType1 = $(this).closest("tr").find("[id^='saleType']").val().trim();


        //    //$('#productsTable tr').each(function () {
        //    //    tblProductId1 = $(this).find(".ProductId").html();
        //    //    if (typeof tblProductId1 != 'undefined') {
        //    //        if (EnteredProductId1 == tblProductId1.trim()) {
        //    //            //alert('abc');
        //    //            tblProductStock1 = $(this).find(".stock").html().trim();
        //    //        }
        //    //    }
        //    //});

        //    for (var i = 0, l = products.length; i < l; i++) {
        //        tblProductId1 = products[i][0];
        //        if (typeof tblProductId1 != 'undefined') {
        //            if (EnteredProductId1 == tblProductId1.trim()) {
        //                tblProductStock1 = products[i][3];
        //            }
        //        }

        //    }

        //    if (Number(EnteredQty1) > Number(tblProductStock1) && saleType1 == "false") {
        //        alert("Item # " + idx1 + " available stock is only " + tblProductStock1);
        //        //$(this).closest("tr").find("[id^='quantity']").val(tblProductStock1);
        //        isFalse = false;
        //        return false;
        //    }
        //});
        //if (isFalse == false) {
        //    return false;
        //}
        //-----------------------------------------------------------------^-product stock check-^--------------------------

        //alert($('#ItemsTotal').val());
        var wentRight = 100;
        var InvalidproductName = ' ';
        var idx = 0;



        if (isNaN($('#total').val())) {
            alert('Total is not valid');
            return false;
        }
        //
        if (isNaN($('#balance').val())) {
            alert('Balance is not valid');
            return false;
        }
        //commented due to new changes sohail
        //$('#selectedProducts > tbody  > tr').each(function () {
        //    idx += 1;
        //    var price = $(this).find("[id^='salePrice']").val();

        //    //alert(price);
        //    if (!price) {
        //        InvalidproductName += $(this).find("[id^='name']").val() + ", ";
        //        //alert(price + " returning");
        //        wentRight = 1;
        //        //return false;

        //    }
        //    if ($(this).find("[id^='name']").val().trim() == "" || $(this).find("[id^='salePrice']").val().trim() == "" || $(this).find("[id^='quantity']").val().trim() == "") {
        //        wentRight = 2;
        //    }
        //});

        //if (wentRight == 0) {
        //    //alert("item # " + idx + " " + InvalidproductName + " is not a valid product name. Please select valid product from product list");
        //    alert("(Item # " + idx + ") " + InvalidproductName + " Please select appropriate product name from list");
        //    return false;
        //}
        if (wentRight == 1) {
            if (confirm('Do you want to add' + InvalidproductName + 'as an service')) {
                return false;
            } else {
                return true;
            }
        }
        if (wentRight == 2) {
            alert('Row is empty')
            return false;
        }
        //var kiaHai = checkAvaiableStock();
        //alert(kiaHai);
        //commented due to new changes sohail
        //if (checkAvaiableStock() == false) return false;

        //if ($('#ItemsTotal').val() == 0) {
        //    alert('Please add at least one product to proceed');
        //    return;
        //}

        if ($('#discount').val().trim() == "") {
            $('#discount').val(0);
        }
        if ($('#paid').val().trim() == "") {
            $('#paid').val(0);
        }
        //if ($('#ItemsTotal').val().trim() == "") {
        //    $('#ItemsTotal').val(0);
        //}
        getAll_AvailableCurrencies();
        var storedTotal = totalVndBalanceHeader;
        if (!isNaN($('#total').val()) || !$('#total').val('0')) {
            //$('#lefttotalvnd').val(storedTotal);
            $('#totalitsmspan').text(storedTotal);
            //set total in header title
            debugger;
            if (storedTotal == 0) {
                return;
            }
            $('#totalitsmspan').text(storedTotal);
            $('#payModal').modal('show');
            // Use the shown.bs.modal event to focus on the input after the modal is fully shown
            $('#payModal').on('shown.bs.modal', function () {
                $('#cashvnd').focus();
            });
            
        }
        else {
            alert('please select product first');
        }
        //$("#CreateSO").attr("disabled", true);
        $('form').preventDoubleSubmission();

    });
    $('.cancelPaymentButton').on('click', function (e) {
        debugger;
        $('#lefttopayvnd').val('0');
        $('#cashvnd').val('0');
        $('#cardvnd').val('0');
        //totalVndBalanceHeader = 0;
        //$('#totalitsmspan').text('0');
        $('#payallbycard').text('Pay all by card');
        //$('#validatepyment').prop('disabled', true);
        $('#payModal').modal('hide'); // Hide the modal
        $('#validatepyment').prop('disabled', true);
    });
    $('.cancelQuantitButton').on('click', function (e) {
        //$('#productQuantity').val('1');
        $('#productQuantity').val('1');
    });
    $('#payallbycard').click(function () {

        //const connection = $.connection.phevaHub;
        //// Start the connection
        ////$.connection.hub.start({ transport: 'longPolling' })
        //const getStoredCookies = getCookie('selectedProducts');
        //$.connection.hub.start({ transport: ['webSockets', 'longPolling', 'serverSentEvents'] })
        //    .done(function () {
        //        console.log('Connection started!');
        //        connection.server.getCustomerProductDetails(getStoredCookies);
        //    })
        //    .fail(function (error) {
        //        console.error('Could not connect to SignalR hub:', error);
        //    });


        //debugger;
        // Get the stored cookie value
        //const getStoredCookies = getCookie('selectedProducts');

        //// Check if the cookie exists
        //if (getStoredCookies) {
        //    // Parse the JSON string to an array of objects
        //    const productsArray = JSON.parse(getStoredCookies);

        //    // Loop through the products array and access the productId for each
        //    productsArray.forEach(product => {
        //        console.log('Product ID:', product.productId);
        //        console.log('Product Name:', product.productName);
        //        // Access other properties like productName, salePrice, etc.
        //    });
        //} else {
        //    console.log('No products found in the cookie.');
        //}

        if (totalAmountForSelectedProduct != 0) {
            debugger;
            $('#lefttopayvnd').val('0');
            $('#cashvnd').val('0');
            $('#cashUsd').val('0');
            $('#cashJpy').val('0');
            $('#changeUsd').val('0');
            $('#changejpy').val('0');
            $('#cardvnd').val(totalAmountForSelectedProduct);
            $('#payallbycard').text('Pay all by card'/*totalAmountForSelectedProduct*/);
            cardVndBalance = totalAmountForSelectedProduct;
            $('#totalitsmspan').text(totalAmountForSelectedProduct);
            $('#validatepyment').prop('disabled', false);
        }
        else {
            $('#lefttopayvnd').val('0');
            $('#cashvnd').val('0');
            $('#cardvnd').val('0');
            $('#payallbycard').text('Pay all by card');
            //$('#validatepyment').prop('disabled', true);
        }
        // Get the text from #CreateSO and remove "Pay " prefix
        //var totalpaymentText = $('#CreateSO').text();

        // Try to get the element by ID

        // Check if the element exists
        //if (updatedSelectedProductTotalElement) {
        //    // If the element exists, get its text content
        //    var updatedSelectedProductTotalElement = selectedProductTotalElement.textContent;

        //} else {
        //    // If the element does not exist, set a default value
        //    var getSelectedProductTotalVnd = '0';
        //}
        //$('#cardvnd').val(getSelectedProductTotalVnd);
        //if (getSelectedProductTotalVnd != 0)
        //    $('#payallbycard').text(getSelectedProductTotalVnd);
        //else
        //    $('#payallbycard').text('Pay all by card');
        //$('#validatepyment').prop('disabled', false);


        //alert(getSelectedProductTotal);
        //$("#totalofAllSelecteProducts").html("₫ " + formatNumberWithDots(totalAmount)); // Assuming '#PayButton' is the button ID

        //////
        //var totalpaymentText = $("#CreateSO").data("amount");
        //var totalpayment = totalpaymentText; // Remove "Pay " prefix

        ////var totalpaymentText = $("#CreateSO").data("amount");
        ////var totalpayment = totalpaymentText.replace('Pay ', '').trim(); // Remove "Pay " prefix

        //// Assign the cleaned value to #cardvnd
        //$('#validatepyment').prop('disabled', false);
        //$('#cardvnd').val(formatNumberWithDots(totalpayment));

        ////

        //var totalpayment = $('#total').val();
        //$('#lefttotalvnd').val('');
    });
    $('#validatepyment').click(function () {
        debugger;
        var cardVndAmount = $('#cardvnd').val();
        var leftToPayVndBalance = $('#lefttopayvnd').val();

        var paymenDetails = {
            CardVndAmount: cardVndAmount,
            CeftToPayVndBalance: leftToPayVndBalance,
        }
        const paymentJSON = JSON.stringify(paymenDetails);
        const connection = $.connection.phevaHub;
        ///Post paymentData
        connection.client.broadcastPaymentDetails = function (payments) {
            console.log('Received payment details in Detail view:', payments);
            $('#paymentDetails').empty(); // assuming you have an element to show payment details
            debugger;
            payments.forEach(function (payment) {
                debugger;
                $('#paymentDetails').append(
                    `<div>
                Card VND Amount: ${formatNumberWithDotss(payment.CardVndAmount)}<br />
                Left to Pay VND Balance: ${formatNumberWithDotss(payment.LeftToPayVndBalance)}
            </div>`
                );
            });
        };
          connection.server.broadcastPaymentDetails(paymentJSON);

        //$.connection.hub.start({ transport: ['webSockets', 'longPolling', , 'serverSentEvents'] })
        //    .done(function () {
        //        console.log('Connection started!');
                
        //    })
        //    .fail(function (error) {
        //        console.error('Could not connect to SignalR hub:', error);
        //    });

        // Make the GET request Working Code
        //$.ajax({
        //    url: '/SOSR/USRLWB', // Ensure this matches your route
        //    type: 'GET',
        //    success: function (response) {
        //        try {
        //            debugger;
        //            // If response is already an object, you can access it directly
        //            var data = response;
        //            var aa = data.Response.Response;
        //            var datas = JSON.parse(aa);
        //            $('#vndCustomerCompany').val(datas.ten_cty);
        //            $('#vndCustomerAddress').val(datas.dia_chi);
        //            $('#taxModal').modal('show');

        //        } catch (error) {
        //            // Handle errors if property access fails
        //            console.error('Error accessing data:', error);
        //        }
        //    }
        //});


        //$.ajax({
        //    url: '/SOSR/USRLWB',
        //    type: 'GET',
        //    success: function (response) {
        //        debugger;
        //        // Handle success
        //        console.log('Success:', response);
        //        //$('#taxModal').modal('show');
        //    },
        //    error: function (xhr, status, error) {
        //        // Handle error
        //        console.error('Error:', status, error);
        //    }
        //});
        //var postData = {
        //    "username": "PHEVA",
        //    "password": "2BM@g0J%5sguJ@",
        //    "ma_dvcs": "VP"
        //};

        //// Make the POST request
        //$.ajax({
        //    url: 'https://0106026495-998.minvoice.pro/api/Account/Login',
        //    type: 'POST',
        //    contentType: 'application/json',
        //    data: JSON.stringify(postData),
        //    success: function (response) {
        //        debugger;
        //        // Handle success
        //        console.log('Success:', response);
        //    },
        //    error: function (xhr, status, error) {
        //        // Handle error
        //        console.error('Error:', status, error);
        //    }
        //});

        $('#taxModal').modal('show');
    });
    // Function to rotate the loader
    function rotateLoader($loader) {
        debugger;
        let rotation = 0;
        const interval = setInterval(function () {
            rotation += 10; // Increment rotation by 10 degrees each time
            $loader.css({
                'transform': 'rotate(' + rotation + 'deg)'
            });
        }, 100); // Adjust speed (100ms for smoother rotation)

        // Return the interval ID so we can stop the rotation later
        return interval;
    }
    debugger;
    $('#customerByTaxCode').on('click', function () {
        var customerTaxCode = $('#vndCustomerVat').val();
        debugger;
        // Disable the button and show the loader
        var $button = $(this);
        $button.prop('disabled', true).find('i').hide(); // Hide the search icon
        var $loader = $button.find('.loader');
        $loader.show(); // Show the loader

        var rotationInterval = rotateLoader($loader); // Start rotating the loader

        // Show the loading text
        $button.find('.loading-text').show();

        // Make the GET request
        $.ajax({
            url: '/SOSR/USRLWB', // Ensure this matches your route
            type: 'GET',
            data: { taxCode: customerTaxCode },
            success: function (response) {
                try {
                    var data = response;
                    var aa = data.Response.Response;
                    var datas = JSON.parse(aa);
                    if (datas.error != undefined) {
                        alert(datas.error);
                        $('#vndCustomerCompany').val('');
                        $('#vndCustomerAddress').val('');
                    }
                    else {
                        $('#vndCustomerCompany').val(datas.ten_cty);
                        $('#vndCustomerAddress').val(datas.dia_chi);
                        $('#taxModal').modal('show');
                    }
                } catch (error) {
                    console.error('Error accessing data:', error);
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX request failed:', status, error);
            },
            complete: function () {
                // Stop the loader rotation
                clearInterval(rotationInterval);

                // Re-enable the button and restore its original state
                $button.prop('disabled', false);
                $button.find('i').show(); // Show the search icon again
                $loader.hide(); // Hide the loader
                $button.find('.loading-text').hide(); // Hide the loading text
            }
        });


    });
    //$('#customerByTaxCode').on('click', function () {
    //    var customerTaxCode = $('#vndCustomerVat').val();
    //    // Make the GET request Working Code
    //    $.ajax({
    //        url: '/SOSR/USRLWB', // Ensure this matches your route
    //        type: 'GET',
    //        data: { taxCode: customerTaxCode },
    //        success: function (response) {
    //            try {
    //                debugger;
    //                // If response is already an object, you can access it directly
    //                var data = response;
    //                var aa = data.Response.Response;
    //                var datas = JSON.parse(aa);
    //                $('#vndCustomerCompany').val(datas.ten_cty);
    //                $('#vndCustomerAddress').val(datas.dia_chi);
    //                $('#taxModal').modal('show');

    //            } catch (error) {
    //                // Handle errors if property access fails
    //                console.error('Error accessing data:', error);
    //            }
    //        }
    //    });

    //});
    //$('#addQuantityModal').click(function (event) { debugger; $('#quantityAddpopup').modal('show'); });
    jQuery.fn.preventDoubleSubmission = function () {
        $(this).on('submit', function (e) {
            var $form = $(this);
            //alert('abc');
            if ($form.data('submitted') === true) {
                // Previously submitted - don't submit again
                e.preventDefault();
            } else {
                // Mark it so that the next submit can be ignored
                $form.data('submitted', true);
            }
        });

        // Keep chainability
        return this;
    };

    //$(document).on("click", "OpenNewCustForm", function () {
    //    $("#dialog-CreateCustomer").dialog("open");
    //});

    //$('#OpenNewCustForm').click(function () {

    //    $("#dialog-CreateCustomer").dialog("open");
    //});

    //$('#btnCreateNewCust').click(function () {

    //    $("#dialog-CreateCustomer").dialog("close");
    //    //$('#idnCustomer').val(CustomerId);
    //    var contents = $("#NewCustomerId").val();
    //    $("#idnCustomer").val(contents);

    //    contents = $("#NewCustomerName").val();

    //    $("#customer").val(contents);

    //    contents = $("#NewCustomerAddress").val();
    //    $("#customerAddress").val(contents);

    //    $("#PreviousBalance").val(0.00);
    //    update_itemTotal();
    //    //alert(contents);
    //});

    //$('[id^="saleType"]').change(function () {
    //    update_itemTotal();
    //});
    // Example: Runs when a button is clicked
    $('.quickAmountAdd').on('click', function () {
        debugger;
        // Step 1: Get the value from .quickAmountAdd
        var aa = $(this).val();

        // Step 2: Remove all spaces and the "₫" character using regex
        var cleanedValue = aa.replace(/\./g, '').replace(/\s+/g, '').replace('₫', '');

        // Step 3: Parse the cleaned value as a number
        var numericValue = parseFloat(cleanedValue) || 0; // Defaults to 0 if not a valid number

        // Step 4: Get the current value of #cashvnd, remove formatting, and parse as a number
        var cashVndValue = $('#cashvnd').val().replace(/\./g, '').replace(/\s+/g, '').replace('₫', '');
        var currentCash = parseFloat(cashVndValue) || 0; // Defaults to 0 if not a valid number

        // Step 5: Calculate the new total
        var newTotal = numericValue + currentCash;

        // Step 6: Format the new total for display purposes
        var formattedTotal = formatNumberWithDots(newTotal);

        // Step 7: Update the #cashvnd input with the new total value
        $('#cashvnd').val(formattedTotal);

        // Step 8: Recalculate the remaining amount to pay
        calculateLeftToPay();
    });
});

//$(document).on('click', '#payallbycard', function () {
//    debugger;
//    var aa = $('#lefttotalvnd').val();
//    alert(aa);
//});

function barcodeEntered(value) {
    //alert("You entered a barcode : " + value);

    var activeControlId = $(document.activeElement).attr("id");
    //alert(activeControlId);

    if (activeControlId.substring(0, 4) != 'name') {
        alert("To scan product barcode, please place cursor in product name text box.");
        return false;
    }

    //var result = $.grep(products, function (e) { return e.Barcode == barcode; });
    //alert('lakdsfjkljs');

    var result = [];
    //if (typeof result === "undefined") {
    //    alert("something is undefined");
    //}

    //for (var i = 0, len = products.length; i < len; i++) {
    //    alert(products[i]);
    //    if (products[i][0] === barcode) {
    //        result = products[i];
    //        break;
    //    }
    //}

    //var abc = productsBarcodes[2];
    //alert(productsBarcodes.length);
    /*alert(value);*/
    for (var i = 0, len = productsBarcodes.length; i < len; i++) {
        //alert(productsBarcodes[i][1]);
        if (productsBarcodes[i][1] === value) {
            /*alert('found');*/
            result = products[i];
            break;
        }
    }


    if (result.length === 0) {
        alert('unfortunately, No item found against this barcode ');
        return false;
    }

    pfound = 0;
    $('#selectedProducts > tbody  > tr').each(function () {

        if ($(this).find("[id^='idn']").val() == result[0]) {
            $('#name' + clickedIdNum).val('');//to remvoe the bar code from textbox
            num = + $(this).find("[id^='quantity']").val() + 1;
            $(this).find("[id^='quantity']").val(num);
            //alert($(this).find("[id^='quantity']").val());
            //$(this).find("[id^='quantity']").val() += 1;
            //alert(ui.item[0]);
            update_itemTotal();
            pfound = 1;
            return false;
        }
    })
    if (pfound == 0) {
        //alert(result);
        $('#name' + clickedIdNum).val(result[1]);
        $('#salePrice' + clickedIdNum).val(result[2]);
        $('#quantity' + clickedIdNum).val(1);
        $('#idn' + clickedIdNum).val(result[0]);
        $('#PerPack' + clickedIdNum).val(result[4]);
        //document.getElementById(clickedTextboxId).focus();
        update_itemTotal();
        $('#addNewRow').trigger('click');
    }

}
//var arr[]=[null];
//function getProductsList(productsList) {
//    debugger
//    arr.push(productsList)
//    JSON.parse(productsList);
//}
function TriggerBodyEvents() {
    //debugger;
    OnTypeName('#name' + txtSerialNum);
    $('#name' + txtSerialNum).on("keyup", function (e) {
        //alert('#name' + txtSerialNum);
        //if (e.keyCode === 13)
        //{
        //    alert('enter');
        //}
        var code = e.keyCode || e.which;

        _keybuffer += String.fromCharCode(code).trim();

        // trim to last 13 characters
        _keybuffer = _keybuffer.substr(-13);

        if (_keybuffer.length == 13) {
            if (!isNaN(parseInt(_keybuffer))) {
                barcodeEntered(_keybuffer);
                _keybuffer = "";
            }
        }
    });

    $('#quantity' + txtSerialNum).keyup(function () {
        update_itemTotal();
    });
    $('#salePrice' + txtSerialNum).keyup(function () {
        update_itemTotal();
    });
    //$('#delete' + txtSerialNum).keyup(function () {
    //    update_itemTotal();
    //});
    $('#delete' + txtSerialNum).keyup(function (event) {
        //alert(txtSerialNum);
        if (event.keyCode == 13) {
            //var focused = document.activeElement;
            //alert('#' + document.activeElement.id);
            //$('#delete' + txtSerialNum).trigger('click');
            //$('#'+focused.id).trigger('click');
            $('#' + document.activeElement.id).trigger('click');
        }
    });
    $('#delete' + txtSerialNum).click(function () {
        //alert(txtSerialNum);
        $(this).parents("tr").remove();
        $("#addNewRow").removeAttr("disabled");
        update_itemTotal();
    });
    //
    //$('#PreviousBalance').keyup(function () {
    //    alert("fff");
    //    update_itemTotal();
    //});

    $('#quantity' + txtSerialNum).keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    $('#saleType' + txtSerialNum).change(function () {
        //var end = this.value;
        //var firstDropVal = $('#saleType').val();
        update_itemTotal();
    });
    $('#isPack' + txtSerialNum).change(function () {
        //if ($('#isPack' + txtSerialNum).val() == "false") {//false=piece true=PerPack
        //    $('#perPack' + txtSerialNum).val(0);
        //}
        update_itemTotal();
    });
    $('#perPack' + txtSerialNum).keyup(function () {
        //var end = this.value;
        //var firstDropVal = $('#saleType').val();
        update_itemTotal();
    });
    //packing0
    $('#individualWithoutVAT').click(function () {
        $('#vndCustomerName').val('Người mua không lấy hóa đơn');
    });

}


function TriggerFooterEvents() {
    $("#discount,#PreviousBalance").keyup(function () {
        //alert("afasf");
        update_itemTotal();
    });

    $("#paid").keyup(function () {
        //alert(_total);
        var paid = $('#paid').val();
        var balance = _total - paid;
        $('#balance').val(balance.toFixed(2));

        if (IsReturn == 'false') {
            $("#CreateSO").html("Pay " + paid);
        }
        else {
            $("#CreateSO").html("Return " + paid);
        }

        //
    });
    // Attach the keyup event handler to both inputs old working by sohail
    //$('#cardvnd, #cashvnd').keyup(function () {
    //    calculateLeftToPay();
    //});


    // Input event listener for customer side (Cash VND, Cash USD, CASH JPY)
    $('#cardvnd, #cashvnd, #cashUsd, #cashJpy').on('input', function () {
        //Temporary commented
        //let inputVal = $(this).val().replace(/[^0-9]/g, ''); // Remove non-numeric characters
        //let formattedVal = formatNumberWithDots(inputVal);
        //$(this).val(formattedVal);

        // Recalculate the remaining amount to pay
        calculateLeftToPay();
    });

    // Input event listener for seller side (Change USD, Change JPY)
    $('#changeUsd, #changejpy').on('input', function () {
        let inputVal = $(this).val().replace(/[^0-9.]/g, ''); // Remove non-numeric characters except the decimal point
        $(this).val(inputVal);

        // Recalculate after the seller enters the change
        calculateLeftToPay();
    });
    //$('#cardvnd, #cashvnd, #cashUsd').on('input', function () {
    $('#cardvnd, #cashvnd').on('input', function () {
        let inputVal = $(this).val().replace(/[^0-9]/g, ''); // Remove non-numeric characters
        let formattedVal = formatNumberWithDots(inputVal);
        $(this).val(formattedVal);

        // Recalculate the remaining amount to pay
        calculateLeftToPay();
    });

    //// Input event listener for seller side (Change USD)
    //$('#changeUsd').on('input', function () {
    //    let inputVal = $(this).val().replace(/[^0-9.]/g, ''); // Remove non-numeric characters except the decimal point
    //    $(this).val(inputVal);

    //    // Recalculate after the seller enters the change
    //    calculateLeftToPay();
    //});


    //$('#cardvnd, #cashvnd, #cashUsd').on('input', function () {
    //    // Format the VND input fields and handle calculations
    //    let inputVal = $(this).val().replace(/[^0-9]/g, '');
    //    let formattedVal = formatNumberWithDots(inputVal);
    //    $(this).val(formattedVal);

    //    // Recalculate the total payment and remaining amount
    //    calculateLeftToPay();
    //});

    //$('#cardvnd, #cashvnd').on('input', function () {
    //    // Remove any non-numeric characters (except for the period which is used for thousands separator)
    //    let inputVal = $(this).val().replace(/[^0-9]/g, '');

    //    // Convert the cleaned value back to an integer, format it, and update the input field
    //    let formattedVal = formatNumberWithDots(inputVal);
    //    $(this).val(formattedVal);
    //    // Recalculate payment totals
    //    calculateLeftToPay();
    //});


    //// Set the exchange rate (USD to VND)
    //var exchangeRate = 24580; // Example rate

    //// Initialize total amount to pay (this could be dynamically set elsewhere)
    //var totalAmountToPay = parseFloat($("#CreateSO").data("amount")) || 0;
    //$('#lefttopayvnd').val(totalAmountToPay);

    //// Handle input in #cashUsd
    //$('#cashUsd').on('input', function () {
    //    // Get the entered USD amount and convert it to VND
    //    var cashUsd = parseFloat($('#cashUsd').val().replace(/,/g, '')) || 0;
    //    var cashInVnd = cashUsd * exchangeRate;

    //    // Update the total amount paid (cash entered by the customer)
    //    var totalPaidInVnd = cashInVnd;

    //    // Calculate the remaining amount to pay
    //    var remainingAmount = totalAmountToPay - totalPaidInVnd;

    //    // Update the remaining amount in #lefttopayvnd
    //    $('#lefttopayvnd').val(remainingAmount);
    //});

    // Handle input in #changeusvnd
    //$('#changeUsd').on('input', function () {
    //    // Get the entered change amount in USD and convert it to VND
    //    var changeUsd = parseFloat($('#changeUsd').val().replace(/,/g, '')) || 0;
    //    var changeInVnd = changeUsd * exchangeRate;

    //    // Calculate the total amount paid (cash + change)
    //    //var cashUsd = parseFloat($('#cashUsd').val().replace(/,/g, '')) || 0;
    //    //var totalPaidInVnd = (cashUsd * exchangeRate) + changeInVnd;
    //    var totalPaidInVnd =  changeInVnd;

    //    // Update the remaining amount to pay
    //    var remainingAmount = totalAmountToPay + totalPaidInVnd;

    //    // Update the remaining amount in #lefttopayvnd
    //    $('#lefttopayvnd').val(remainingAmount);
    //});
    //// Set the exchange rate
    //var exchangeRate = 24580; // USD to VND

    //// Set the total amount to pay (can be updated dynamically)
    //var totalAmountToPay = parseFloat($("#CreateSO").data("amount")) || 0;
    //$('#lefttopayvnd').val(totalAmountToPay);

    //// Handle input in #cashUsd
    //$('#cashUsd').on('input', function () {
    //    // Get the value entered in USD and convert to VND
    //    var cashUsd = parseFloat($('#cashUsd').val().replace(/,/g, '')) || 0;
    //    var cashInVnd = cashUsd * exchangeRate;

    //    // Calculate the remaining amount to pay
    //    var remainingAmount = totalAmountToPay - cashInVnd;

    //    // Update the remaining amount in VND
    //    $('#lefttopayvnd').val(remainingAmount);

    //    // Calculate and display change if the remaining amount is negative
    //    if (remainingAmount < 0) {
    //        var changeToGive = Math.abs(remainingAmount);
    //        $('#changeToGive').text('Change to return: ' + changeToGive.toLocaleString() + ' VND');
    //    } else {
    //        $('#changeToGive').text('');
    //    }
    //});

    //// Handle input in #changeusvnd
    //$('#changeUsd').on('input', function () {
    //    // Get the value entered in USD for change and convert to VND
    //    var changeUsd = parseFloat($('#changeUsd').val().replace(/,/g, '')) || 0;
    //    var changeInVnd = changeUsd * exchangeRate;

    //    // Recalculate the remaining amount after considering the change
    //    var cashUsd = parseFloat($('#cashUsd').val().replace(/,/g, '')) || 0;
    //    var cashInVnd = cashUsd * exchangeRate;
    //    var totalPaidInVnd = cashInVnd + changeInVnd;
    //    var remainingAmount = totalAmountToPay - totalPaidInVnd;

    //    // Update the remaining amount in VND
    //    $('#lefttopayvnd').val(remainingAmount);

    //    // Calculate and display change if the remaining amount is negative
    //    if (remainingAmount < 0) {
    //        var changeToGive = Math.abs(remainingAmount);
    //        $('#lefttopayvnd').text('Change to return: ' + changeToGive.toLocaleString() + ' VND');
    //    } else {
    //        $('#lefttopayvnd').text('');
    //    }
    //});

    //$('#cashUsd').on('input', function () {
    //    // Remove any non-numeric characters (except for the period which is used for thousands separator)
    //    // Fetch the total amount to pay (if it's dynamically updated elsewhere, you should get it freshly)
    //    var totalAmounts = $("#CreateSO").data("amount");

    //    // Update the total amounts in the input field
    //    $('#lefttopayvnd').val(totalAmounts);

    //    // Fetch the entered USD amount and the total VND amount to pay
    //    var cashUsd = parseFloat($('#cashUsd').val().replace(/,/g, '')) || 0; // Removing commas if present
    //    var totalAmountPaid = parseFloat($('#lefttopayvnd').val()) || 0;

    //    // Perform the calculation: Convert USD to VND (assuming 4624 is the exchange rate) and subtract from totalAmountPaid
    //    var aaa = cashUsd * 24580;
    //    var totalRemaining = totalAmountPaid - (cashUsd * 24580);
    //    $('#totalDollarsToVnd').text(aaa);

    //    // Update the 'lefttopayvnd' with the recalculated total remaining amount
    //    $('#lefttopayvnd').val(totalRemaining);
    //});
    //$('#changeusvnd').on('input', function () {
    //    // Fetch the entered USD amount and the total VND amount to pay
    //    var totalAmountPaid = parseFloat($('#lefttopayvnd').val()) || 0;
    //    var totalRemaining = (totalAmountPaid + ($('#changeusvnd').val() * 24580));
    //    // Update the 'lefttopayvnd' with the recalculated total remaining amount
    //    $('#lefttopayvnd').val(totalRemaining);
    //});
    //$('#cashJpy').on('input', function () {
    //    // Remove any non-numeric characters (except for the period which is used for thousands separator)
    //    let inputVal = $(this).val().replace(/[^0-9]/g, '');

    //    // Convert the cleaned value back to an integer, format it, and update the input field
    //    let formattedVal = formatNumberWithDots(inputVal);
    //    $(this).val(formattedVal);
    //    // Recalculate payment totals
    //    calculateLeftToPay();
    //});



    //set value of Pay Modal form
    //$("#cardvnd").keyup(function () {
    //    debugger;
    //    //alert(_total);
    //    var cdVnd = $('#cardvnd').val();
    //    var totalpaymentText = $('#CreateSO').text();
    //    var totalpayment = totalpaymentText.replace('Pay ', '').trim();
    //    var leftToPayVndBalance = totalpayment - cdVnd;
    //    var cashVndBalance = leftToPayVndBalance - cdVnd;
    //    $('#lefttopayvnd').val(leftToPayVndBalance.toFixed(2));
    //    leftVndBalance = $('#lefttopayvnd').val();
    //    totalBillPaid = cdVnd;
    //    //console.log('#cardvnd' + cdVnd);
    //    //console.log(leftVndBalance);
    //    //if (IsReturn == 'false') {
    //    //    $("#CreateSO").html("Pay " + paid);
    //    //}
    //    //else {
    //    //    $("#CreateSO").html("Return " + paid);
    //    //}

    //    //
    //});

    let focusedInput = null;  // Store the currently focused input field eg. cardvn / cashvnd
    $('#quantityAddpopup').on('focus', 'input', function () {
        focusedInput = $(this);
    });

    // When any input inside the modal is focused, set it as the focused input
    $('#payModal').on('focus', 'input', function () {
        focusedInput = $(this);
    });


    // Function to append a number to the currently focused input field Calculator
    window.appendNumber = function (number) {
        if (focusedInput) {  // Check if an input is focused
            var currentVal = focusedInput.val();  // Get the current value of the input field

            // If the current value is "0", replace it with the new number
            if (currentVal === '0') {
                focusedInput.val(number);
            } else {
                // Otherwise, append the new number to the current value
                focusedInput.val(currentVal + number);
            }

            // Trigger the input event to reformat and recalculate
            focusedInput.trigger('input');
        }
    };
    //Calculator Clear input
    window.clearInput = function () {
        if (focusedInput) {  // Check if an input is focused
            focusedInput.val('0');  // Clear the input field
            focusedInput.trigger('input');  // Trigger the input event to update calculations
        }
    };

    // Function to delete the last character from the focused input field Calculator
    window.deleteLast = function () {
        if (focusedInput) {  // Check if an input is focused
            var currentVal = focusedInput.val().replace(/\./g, '');  // Remove dots to get the plain number string
            if (currentVal.length > 1) {
                focusedInput.val(formatNumberWithDots(currentVal.slice(0, -1)));  // Remove the last character and reformat
            } else {
                focusedInput.val('0');  // If only one digit remains, set to "0"
            }

            focusedInput.trigger('input');  // Trigger the input event to update calculations
        }
    };
    // Function to append number
    window.appendQuantityNumber = function (number) {
        if (focusedInput) { // Check if an input is focused
            var currentVal = focusedInput.val(); // Get the current value of the input field

            // If it's the first entry, replace the current value with the new number
            if (isFirstEntry) {
                focusedInput.val(number); // Replace with the new number
                isFirstEntry = false; // Set the flag to false after the first entry
            } else {
                // Otherwise, append the new number to the current value
                focusedInput.val(currentVal + number);
            }

            // Trigger the input event to reformat and recalculate
            focusedInput.trigger('input');
        }
    };
    //window.appendQuantityNumber = function (number) {
    //    if (focusedInput) {  // Check if an input is focused
    //        var currentVal = focusedInput.val();  // Get the current value of the input field

    //        // If the current value is "0", replace it with the new number
    //        if (currentVal === '0') {
    //            focusedInput.val(number);
    //        } else {
    //            // Otherwise, append the new number to the current value
    //            focusedInput.val(currentVal + number);
    //        }

    //        // Trigger the input event to reformat and recalculate
    //        focusedInput.trigger('input');
    //    }
    //};
    window.clearQuantityInput = function () {
        if (focusedInput) {  // Check if an input is focused
            focusedInput.val('0');  // Clear the input field
            focusedInput.trigger('input');  // Trigger the input event to update calculations
        }
    };

    // Function to delete the last character from the focused input field Calculator
    window.deleteQuantityLast = function () {
        if (focusedInput) {  // Check if an input is focused
            var currentVal = focusedInput.val().replace(/\./g, '');  // Remove dots to get the plain number string
            if (currentVal.length > 1) {
                focusedInput.val(formatNumberWithDots(currentVal.slice(0, -1)));  // Remove the last character and reformat
            } else {
                focusedInput.val('0');  // If only one digit remains, set to "0"
            }

            focusedInput.trigger('input');  // Trigger the input event to update calculations
        }
    };

}



// This function simulates receiving a barcode scan input Sohail


///
// Attach the event handler using the correct method
function showHiddenInputs() {
    var checkbox = document.getElementById('payInMultipleCurrencies');
    var hiddenInputs = document.querySelectorAll('.hiddenCurrencyInputs');

    // Toggle display based on checkbox state
    hiddenInputs.forEach(function (input) {
        input.style.display = checkbox.checked ? 'block' : 'none';
    });
}
//function calculateLeftToPay() {
//    //debugger;
//    // Get values for card payment, cash payment, and total payment
//    //var cdVnd = parseFloat($('#cardvnd').val()) || 0;  // Card payment
//    //var cashVnd = parseFloat($('#cashvnd').val()) || 0; // Cash payment
//    //var totalpaymentText = $("#CreateSO").data("amount"); // Total payment amount from data attribute
//    //var totalpayment = parseFloat(totalpaymentText) || 0; // Parse to number


//    // Get values for card payment, cash payment, and total payment
//    var cdVnd = parseFloat($('#cardvnd').val().replace(/\./g, '')) || 0;  // Remove dots and convert to number
//    var cashVnd = parseFloat($('#cashvnd').val().replace(/\./g, '')) || 0; // Remove dots and convert to number
//    var totalpaymentText = $("#CreateSO").data("amount"); // Total payment amount from data attribute
//    var totalpayment = parseFloat(totalpaymentText) || 0; // Parse to number

//    //var cardVndBalance;
//    //var cashVndBalance;

//    //var totalPayableBill;
//    //var totalBillPaid;
//    //var leftVndBalance;


//    // Calculate the remaining amount to pay
//    cardVndBalance = cdVnd;
//    cashVndBalance = cashVnd;
//    leftVndBalance = totalpayment - (cdVnd + cashVnd);
//    // Update the value for left to pay field
//    $('#lefttopayvnd').val(formatNumberWithDots(leftVndBalance));
//    // Calculate total amount paid
//    totalBillPaid = cdVnd + cashVnd;
//    // Check if total paid matches total amount
//    if (totalBillPaid === totalpayment || totalBillPaid > totalpayment) {
//        $('#validatepyment').prop('disabled', false); // Enable the button if fully paid
//    } else {
//        $('#validatepyment').prop('disabled', true); // Disable the button if not fully paid
//    }

//    // Optional UI updates
//    // if (IsReturn == 'false') {
//    //     $("#CreateSO").html("Pay " + leftToPayVndBalance.toFixed(2));
//    // } else {
//    //     $("#CreateSO").html("Return " + leftToPayVndBalance.toFixed(2));
//    // }
//}
function calculateLeftToPay() {
    debugger;
    // Get VND values for card and cash payments from the customer (remove dots and convert to numbers)
    var cdVnd = parseFloat($('#cardvnd').val().replace(/\./g, '')) || 0;
    var cashVnd = parseFloat($('#cashvnd').val().replace(/\./g, '')) || 0;
    cardVndBalance = cdVnd;
    cashVndBalance = cashVnd;
    // Get the USD payment entered by the customer
    var cashUsd = parseFloat($('#cashUsd').val()) || 0;
    var cashUsdInVnd = cashUsd * usdToVndRate; // Convert USD to VND
    //$('#totalDollarsToVnd').text(cashUsdInVnd.toLocaleString('en-US'));
    var formattedUsdToVndValue = formatNumberWithDots(Math.floor(cashUsdInVnd)); // Use Math.floor to avoid decimals
    // Step 4: Set the formatted value to #totalDollarsToVnd
    $('#totalDollarsToVnd').text(formattedUsdToVndValue);

    // Get the JPY payment entered by the customer
    var cashJpy = parseFloat($('#cashJpy').val()) || 0;
    var cashJpyInVnd = cashJpy * jpyToVndRate; // Convert JPY to VND
    //$('#totalYensToVnd').text(cashJpyInVnd.toLocaleString('en-US'));
    var formattedYensToVndValue = formatNumberWithDots(Math.floor(cashJpyInVnd)); // Use Math.floor to avoid decimals
    // Step 4: Set the formatted value to #totalDollarsToVnd
    $('#totalYensToVnd').text(formattedYensToVndValue);

    // Get the USD change entered by the seller
    var changeUsd = parseFloat($('#changeUsd').val()) || 0;
    var changeUsdInVnd = changeUsd * usdToVndRate; // Convert USD to VND

    // Get the JPY change entered by the seller
    var changeJpy = parseFloat($('#changejpy').val()) || 0;
    var changeJpyInVnd = changeJpy * jpyToVndRate; // Convert JPY to VND

    // Get the total payment amount (from the data attribute)
    var totalPaymentText = $("#CreateSO").data("amount");
    var totalPayment = parseFloat(totalPaymentText) || 0;

    // Calculate total amount paid by the customer (Card + Cash VND + Cash USD in VND + Cash JPY in VND)
    //var totalBillPaid = cdVnd + cashVnd + cashUsdInVnd + cashJpyInVnd;
    totalBillPaid = cdVnd + cashVnd + cashUsdInVnd + cashJpyInVnd;

    // Calculate the remaining balance (Customer side payment minus total amount)
    //var leftVndBalance = totalPayment - totalBillPaid;
    leftVndBalance = totalPayment - totalBillPaid;

    // Now apply the seller's ChangeUsd and ChangeJpy to adjust the remaining balance (if negative)
    leftVndBalance += changeUsdInVnd + changeJpyInVnd; // Add seller's contribution in both USD and JPY

    // If the remaining balance is negative or zero, update lefttopayvnd accordingly
    $('#lefttopayvnd').val(formatNumberWithDots(leftVndBalance));

    // Enable or disable the payment button based on whether the balance is fully paid or overpaid
    if (leftVndBalance <= 0) {
        $('#validatepyment').prop('disabled', false); // Enable the button if fully paid or overpaid
    } else {
        $('#validatepyment').prop('disabled', true);  // Disable the button if not fully paid
    }
}
//function calculateLeftToPay() {
//    // Get VND values for card and cash payments from the customer (remove dots and convert to numbers)
//    var cdVnd = parseFloat($('#cardvnd').val().replace(/\./g, '')) || 0;
//    var cashVnd = parseFloat($('#cashvnd').val().replace(/\./g, '')) || 0;

//    // Get the USD payment entered by the customer
//    var cashUsd = parseFloat($('#cashUsd').val()) || 0;
//    var cashUsdInVnd = cashUsd * usdToVndRate; // Convert USD to VND
//    $('#totalDollarsToVnd').text(cashUsdInVnd);

//    // Get the USD change entered by the seller
//    var changeUsd = parseFloat($('#changeUsd').val()) || 0;
//    var changeUsdInVnd = changeUsd * usdToVndRate; // Convert USD to VND

//    // Get the total payment amount (from the data attribute)
//    var totalPaymentText = $("#CreateSO").data("amount");
//    var totalPayment = parseFloat(totalPaymentText) || 0;

//    // Calculate total amount paid by the customer (Card + Cash VND + Cash USD in VND)
//    var totalBillPaid = cdVnd + cashVnd + cashUsdInVnd;

//    // Calculate the remaining balance (Customer side payment minus total amount)
//    var leftVndBalance = totalPayment - totalBillPaid;

//    // Now apply the seller's ChangeUsd to adjust the remaining balance (if negative)
//    leftVndBalance += changeUsdInVnd; // Add the seller's contribution in VND

//    // If the remaining balance is negative or zero, update lefttopayvnd accordingly
//    if (leftVndBalance < 0) {
//        $('#lefttopayvnd').val(formatNumberWithDots(leftVndBalance)); // Display the remaining negative amount
//    } else {
//        $('#lefttopayvnd').val(formatNumberWithDots(leftVndBalance)); // If positive, show the remaining amount
//    }

//    // Enable or disable the payment button based on whether the balance is fully paid or overpaid
//    if (leftVndBalance <= 0) {
//        $('#validatepyment').prop('disabled', false); // Enable the button if fully paid or overpaid
//    } else {
//        $('#validatepyment').prop('disabled', true);  // Disable the button if not fully paid
//    }
//} 

//function calculateLeftToPay() {
//    // Get VND values for card and cash payments (after removing any dots)
//    var cdVnd = parseFloat($('#cardvnd').val().replace(/\./g, '')) || 0;
//    var cashVnd = parseFloat($('#cashvnd').val().replace(/\./g, '')) || 0;

//    // Get the USD payment and convert to VND
//    var cashUsd = parseFloat($('#cashUsd').val()) || 0;
//    var cashUsdInVnd = cashUsd * usdToVndRate;

//    // Get the total payment amount from the data attribute
//    var totalPaymentText = $("#CreateSO").data("amount");
//    var totalPayment = parseFloat(totalPaymentText) || 0;

//    // Calculate the total paid amount in VND
//    var totalBillPaid = cdVnd + cashVnd + cashUsdInVnd;

//    // Calculate the remaining amount to pay in VND
//    var leftVndBalance = totalPayment - totalBillPaid;

//    // Update the Left to Pay VND field
//    $('#lefttopayvnd').val(formatNumberWithDots(leftVndBalance));

//    // If the total paid amount is more than the bill, calculate change in USD
//    if (totalBillPaid > totalPayment) {
//        var excessVnd = totalBillPaid - totalPayment;
//        var changeUsd = excessVnd / usdToVndRate;
//        $('#changeUsd').val(changeUsd.toFixed(2)); // Show change in USD
//    } else {
//        $('#changeUsd').val('0'); // No change required
//    }

//    // Enable/Disable payment button based on whether the bill is fully paid
//    if (totalBillPaid >= totalPayment) {
//        $('#validatepyment').prop('disabled', false); // Enable the button
//    } else {
//        $('#validatepyment').prop('disabled', true); // Disable the button
//    }
//}


//function calculateLeftToPay() {
//    var cdVnd = parseFloat($('#cardvnd').val()) || 0;  // Card payment
//    var cashVnd = parseFloat($('#cashvnd').val()) || 0; // Cash payment
//    //var totalpaymentText = $('#CreateSO').text();

//    var totalpaymentText = $("#CreateSO").data("amount");
//    //var totalpaymentText = $("#totalofAllSelecteProducts").data("amounts");
//    var totalpayment = parseFloat(totalpaymentText) || 0;

//    //var totalpaymentText = $("#CreateSO").data("amount");
//    //var totalpayment = parseFloat(totalpaymentText.replace('Pay ', '').trim()) || 0;

//    // Calculate the remaining amount to pay
//    var leftToPayVndBalance = totalpayment - (cdVnd + cashVnd);
//    cardVndBalance = cdVnd;
//    cashVndBalance = cashVnd;
//    //console.log('cardVndBalance : ' + cardVndBalance + '' + 'cashVndBalance : ' + cashVndBalance);

//    //$('#lefttopayvnd').val(leftToPayVndBalance.toFixed(2));
//    $('#lefttopayvnd').val(leftToPayVndBalance);
//    leftVndBalance = $('#lefttopayvnd').val();
//    totalBillPaid = cdVnd + cashVnd;  // Total amount paid


//    // Optionally, update other parts of the UI
//    // if (IsReturn == 'false') {
//    //     $("#CreateSO").html("Pay " + leftToPayVndBalance.toFixed(2));
//    // } else {
//    //     $("#CreateSO").html("Return " + leftToPayVndBalance.toFixed(2));
//    // }
//}
function ConfigDialogueCreateCustomer() {
    //alert("create customer configured");
    $("#dialog-CreateCustomer").dialog({
        title: '',
        modal: true,
        autoOpen: false,
        resizable: true,
        draggable: true,
        height: '480',
        width: '600',
        closeOnEscape: true,
        //position: {
        //    my: 'left top',
        //    at: 'left bottom',
        //    of: $(param)
        //},
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close", ui.dialog | ui).show();
            $('#dialog-CreateCustomer').css('overflow', 'hidden'); //this line does the actual hiding
        }


    });

}
$('td:first-child').each(function () {
    console.log($(this).text());
});

function checkAvaiableStock() {
    //alert('abc');
    var EnteredQty1 = 0;
    var EnteredProductId1 = 0;
    var tblProductStock1 = 0;
    var tblProductId1 = 0;
    var tblProductName1 = "";
    var saleType1 = false;
    var idx1 = 0;
    var isFalse = true;
    $('#selectedProducts tbody tr').each(function () {

        //alert(clickedIdNum + ',' + idx1);
        //if (clickedIdNum != idx1) {
        //    idx1 += 1;
        //    return true;
        //}
        EnteredQty1 = $(this).closest("tr").find("[id^='quantity']").val();
        EnteredProductId1 = $(this).closest("tr").find("[id^='idn']").val().trim();
        saleType1 = $(this).closest("tr").find("[id^='saleType']").val().trim();

        for (var i = 0, l = products.length; i < l; i++) {
            tblProductId1 = products[i][0];
            if (typeof tblProductId1 != 'undefined') {
                if (EnteredProductId1 == tblProductId1.trim()) {
                    tblProductStock1 = products[i][3];
                    tblProductName1 = products[i][1];
                }
            }
        }

        if (Number(EnteredQty1) > Number(tblProductStock1) && saleType1 == "false") {
            alert("Item '" + tblProductName1 + "' available stock is " + tblProductStock1);
            isFalse = false;
            //alert(isFalse);
            return isFalse;
        }
        else {
            isFalse = true;
        }

    });
    return isFalse;
}
function update_itemTotal() {


    //alert('1234');

    var ItemsTotal = 0;

    var orderQty = 0;
    var orderQtyPiece = 0;
    var returnQty = 0;
    var returnQtyPiece = 0;
    var SN = 0;

    //$("#OrderTotal").val('Order Total(' + parseInt(orderQty) + ')');

    //$('#selectedProducts > tbody  > tr').each(function () {

    //    //$(this).find("[id^='isPack']").prop('disabled', true);
    //    //$(this).find("[id^='isPack']").prop('selectedIndex', 1);

    //    if (IsReturn == 'false') {//if (IsReturn == 'True') { c# model uses 'T'rue and java script user 't'rue
    //        //$('#name').prop('selectedIndex', 0);

    //        $(this).find("[id^='saleType']").prop('selectedIndex', 0);
    //    }
    //    else {

    //        $(this).find("[id^='saleType']").prop('selectedIndex', 1);
    //    }

    //    SN += 1;
    //    $(this).find("[id^='SNo']").text(SN);

    //    var qty = 0;
    //    var price = 0;
    //    var perPack = 0;
    //    //alert(IsReturn);

    //    //alert($(this).find("[id^='saleType']").val());
    //    //if ($(this).find("[id^='saleType']").val() == "false") {//sale order
    //    if (IsReturn == 'false') {
    //        //Sale

    //        qty = $(this).find("[id^='quantity']").val();

    //        if (!(qty)) { qty = 0; }
    //        price = $(this).find("[id^='salePrice']").val();

    //        var itemAmount = (qty * price);

    //        if ($(this).find("[id^='isPack']").val() == "true") {//false=item true=PerPack
    //            perPack = $(this).find("[id^='perPack']").val();
    //            if (perPack == 0) perPack = 1;
    //            itemAmount = itemAmount * perPack;
    //            orderQty += parseInt(qty);
    //            //alert(itemAmount);
    //            //alert("pack true");
    //            //$(this).find("[id^='perPack']").val($(this).find("[id^='perPackHidden']").val());
    //            //$(this).find("[id^='perPackHidden']").remove();
    //        }
    //        else {
    //            //alert("pack false");
    //            orderQtyPiece += parseInt(qty);
    //            //$("[id^='perPack']").hide();
    //            //$("[id^='perPack']").after('<input type="hidden" id="perPackHidden' + txtSerialNum + '"  value="' + $("#perPack").val() + '" />').val("").attr("disabled", true);
    //        }

    //        ItemsTotal += itemAmount;
    //        $(this).find("[id^='itemTotal']").val(itemAmount.toFixed(2));
    //    } else {
    //        //Return
    //        //alert($('#saleType').val());
    //        qty = $(this).find("[id^='quantity']").val();
    //        if (!(qty)) { qty = 0; }
    //        price = $(this).find("[id^='salePrice']").val();

    //        var ItemAmount = (qty * price);

    //        if ($(this).find("[id^='isPack']").val() == "true") {//false=item true=PerPack
    //            perPack = $(this).find("[id^='perPack']").val();
    //            if (perPack == 0) perPack = 1;
    //            ItemAmount = ItemAmount * perPack;
    //            orderQty += parseInt(qty);
    //            //alert(ItemAmount);
    //        } else {
    //            orderQtyPiece += parseInt(qty);
    //        }


    //        ItemsTotal += ItemAmount;
    //        //alert(ItemsTotal);
    //        $(this).find("[id^='itemTotal']").val(ItemAmount.toFixed(2));


    //    }

    //    /*$("#OrderTotal").val('Order Total(' + parseInt(orderQty) + ' pack, ' + parseInt(orderQtyPiece) + ' piece)');*/
    //    $("#OrderTotal").val('Order Total(' + parseInt(orderQty) + ')');

    //});

    ////checkAvaiableStock();

    ////$('[id^="name"]').focusout(function () {
    ////    alert("hello");

    ////});
    ////$('#OpenNewCustForm').click(function () {
    ////    $("#dialog-CreateCustomer").dialog("open");
    ////});

    ////$('[id^="quantity"]').change(function () {
    ////    alert("");
    ////});

    ////jQuery('[id^="quantity"]').on('change input propertychange paste', function () {
    ////    alert("c");
    ////});

    ///////////////////////////////////
    //////$('[id^="quantity"]').blur(function () {
    //////var inputObj = this;
    ////var EnteredQty = 0;
    ////var EnteredProductId = 0;
    ////var tblProductStock = 0;
    ////var tblProductId = 0;
    ////var saleType = false;
    ////EnteredQty = this.value.trim();
    //////alert(this.value.trim());
    ////EnteredProductId = $(this).closest("tr").find("[id^='idn']").val().trim();
    ////saleType = $(this).closest("tr").find("[id^='saleType']").val().trim();


    //////$('#productsTable tr').each(function () {
    //////    tblProductId = $(this).find(".ProductId").html();

    //////    if (typeof tblProductId != 'undefined') {
    //////        if (EnteredProductId == tblProductId.trim()) {
    //////            //alert('abc');
    //////            tblProductStock = $(this).find(".stock").html().trim();
    //////            //alert(tblProductStock.trim());
    //////            //if (EnteredQty.trim() > tblProductStock.trim()) {
    //////            //    alert("available stock is " + tblProductStock);
    //////            //    return false; 
    //////            //}
    //////            //return false;
    //////        }
    //////    }
    //////});
    ////for (var i = 0, l = products.length; i < l; i++) {
    ////    tblProductId = products[i][0];
    ////    if (typeof tblProductId != 'undefined') {
    ////        if (EnteredProductId == tblProductId.trim()) {
    ////            tblProductStock = products[i][3];
    ////        }
    ////    }

    ////}
    //////alert(EnteredQty.trim());
    //////alert(tblProductStock.trim());
    ////if (Number(EnteredQty) > Number(tblProductStock) && saleType == "false") {

    ////    alert("available stock is " + tblProductStock);
    ////    $(this).closest("tr").find("[id^='quantity']").val(EnteredQty.trim());
    ////    //return false;

    ////}

    //////$('#productsTable .ProductIdd').each(function () {
    //////    alert($(this).html());
    //////});

    //////});
    ///////////////////////////////////////




    ////$('[id^="saleType"]').change(function () {
    ////    //var end = this.value;
    ////    //var firstDropVal = $('#saleType').val();
    ////    update_itemTotal();
    ////});

    ////$('[id^="quantity"]').change(function () {
    ////    //var end = this.value;
    ////    //var firstDropVal = $('#saleType').val();

    ////    update_itemTotal();
    ////});


    ////jQuery('[id^="quantity"]').on('input propertychange paste', function () {
    ////    alert("qunatity was entered");
    ////});

    ////if (ItemsTotal > 0 || ReturnsTotal > 0) {
    ////    $('#dvCalculations').show();
    ////} else {
    ////    $('#dvCalculations').hide();
    ////}
    //debugger;
    //$('#ItemsTotal').val(ItemsTotal.toFixed(2));//for input element


    //var discount = $('#discount').val();
    //var prevBal = parseFloat($("#PreviousBalance").val());
    //var subtotal = ItemsTotal - discount;
    //var total = 0;
    //if (IsReturn == 'false') {
    //    total = subtotal + prevBal;
    //    $('#pb3').val('Prev. Balance');
    //}
    //else {
    //    total = subtotal - prevBal;
    //    $('#pb3').val('Prev. Balance                                 -');
    //}

    ////total += $("#PreviousBalance").val();
    ////alert(subtotal);
    //$('#subtotal').val(subtotal.toFixed(2));
    //$('#total').val(total.toFixed(2));
    //$('#paid').val(total.toFixed(2));
    ////$('#cardvnd').val(total.toFixed(2));

    //_total = total;
    ////alert(ItemsTotal + ", " + discount + ", " + ReturnsTotal + ", " + prevBal);
    //var paid = $('#paid').val();
    //var balance = balance = _total - paid;
    //$('#balance').val(balance.toFixed(2));
    //$('#lefttopayvnd').val(balance.toFixed(2));
    ///*alert(IsReturn);*/
    //if (IsReturn == 'false') {
    //    $("#CreateSO").html("Pay " + paid);
    //}
    //else {
    //    $("#CreateSO").html("Return " + paid);
    //}
    ////$('#ItemsTotal > tbody > tr > td').val(ItemsTotal);
    ////just update the total to sum
    ////$('.total').text(ItemsTotal);
    //debugger
    ////var totalItem = $('#total').val();
    //var totalItem = $('#total').val();

    //// Convert the string value to a number
    //var totalItemNumber = parseFloat(totalItem);

    //// Check if the conversion was successful and the value is a number
    //if (!isNaN(totalItemNumber)) {
    //    // Use toFixed to format the number to 2 decimal places
    //    var formattedTotalItem = totalItemNumber.toFixed(2);
    //    // Store the formatted value in localStorage
    //    localStorage.setItem('totalItems', formattedTotalItem);
    //} else {
    //    console.error('Invalid number: ' + totalItem);
    //}
}

//openPayModal(){
//    $('#payModal').modal('show');
//}


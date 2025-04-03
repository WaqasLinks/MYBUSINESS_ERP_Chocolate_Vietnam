var productColumns = [{ name: 'Id', minWidth: '100px' }, { name: 'Product', minWidth: '320px' }, { name: 'Purch. Price', minWidth: '100px' }, { name: 'Stock', minWidth: '70px' }, { name: 'PerPack', minWidth: '70px' }, { name: 'Unit', minWidth: '70px' }];
//var products = []; //[['Ciplet', '10', '60'], ['Gaviscon', '85', '12'], ['Surficol', '110', '8']];
var products = new Array();

var supplierColumns = [
    { name: 'Id', minWidth: '100px' },
    { name: 'Name', minWidth: '320px' },
    { name: 'Address', minWidth: '200px' },
    { name: 'Balance', minWidth: '70px' }
];

var suppliers = new Array();
var txtSerialNum = 0;

function OnTypeSupplierName(param) {
    $(param).mcautocomplete({
        showHeader: true,
        columns: supplierColumns,
        source: suppliers,
        select: function (event, ui) {
            alert("Supplier selected:", ui.item); // Debugging

            this.value = (ui.item ? ui.item[1] : '');
            $('#idnSupplier').val(ui.item ? ui.item[0] : '');
            $('#supplierAddress').val(ui.item ? ui.item[2] : '');
            $('#PreviousBalance').val(ui.item ? ui.item[3] : '');

            var supplierId = ui.item ? ui.item[0] : '';
            alert("Selected supplierId:", supplierId); // Debugging

            if (supplierId) {
                $('#idnSupplier').trigger('change'); // Ensure input updates
                fetchLastOrderProducts(supplierId);
            } else {
                console.error("Supplier ID is missing!");
            }

            return false;
        }
    });
}

// Function to Fetch Last Order Products Based on Supplier ID
// Function to Fetch Last Order Products Based on Supplier ID
function fetchLastOrderProducts(supplierId) {
    alert("Fetching last order products for supplierId:", supplierId); // Debugging

    $.ajax({
        url: '/POPRReciver/GetLastOrderProducts',
        type: 'GET',
        data: { supplierId: supplierId },
        dataType: 'json',
        success: function (data) {
            console.log("AJAX success triggered!");
            alert("Success");
            console.log("Raw response from server:", data); // Log the server response

            // Check if data is an array
            if (!Array.isArray(data)) {
                alert("Error: Server did not return an array!");
                console.log("Received:", data);
                return;
            }

            if (data.length > 0) {
                console.log("Calling populateProductTable with data:", data); // Log before calling
                alert("Calling populateProductTable with data: " + JSON.stringify(data)); // Debugging
                populateProductTable(data);
            } else {
                alert('No previous orders found for this supplier.');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching order products:', status, error);
            console.log('Response Text:', xhr.responseText); // Check the response text
        }
    });
    
}

// Function to populate Product Name, Quantity, and Unit fields
//function populateProductTable(products) {
//    console.log("Inside populateProductTable function");  // Log to check if the function is being called
//    alert("Populating product fields:", products);

//    if (!Array.isArray(products)) {
//        console.error("Error: Data is not an array!", products);
//        return;
//    }

//    let productTableBody = $("#selectedProduct tbody"); // Ensure the correct table ID is used
//    productTableBody.empty(); // Clear existing rows

//    products.forEach((product, index) => {
//        let rowNumber = index;

//        let newRow = `
//    <tr>
//        <td>${rowNumber + 1}</td>
//        <td><input type="hidden" name="PurchaseReceiverOrderDetail.Index" value="${rowNumber}" /></td>
//        <td><input type="text" readonly class="form-control" name="PurchaseReceiverOrderDetail[${rowNumber}].ProductId" id="idn${rowNumber}" value="${product.ProductName}" /></td>
//        <td><input type="text" autocomplete="off" class="form-control" name="name${rowNumber}" id="name${rowNumber}" placeholder="Type product name" data-toggle="tooltip" data-placement="top" title="Type product name" value="${product.ProductName}"></td>
//        <td><input type="text" class="form-control" name="PurchaseReceiverOrderDetail[${rowNumber}].Quantity" id="quantity${rowNumber}" value="${product.Quantity}" /></td>
//        <td><input type="text" class="form-control" name="PurchaseReceiverOrderDetail[${rowNumber}].Unit" id="unit${rowNumber}" value="${product.Unit}" /></td>
//        <td><button type="button" id="delete${rowNumber}" class="delete btn btn-default add-new"><i class="material-icons">&#xE872;</i></button></td>
//    </tr>
//`;

//        productTableBody.append(newRow);
//    });

//    console.log("Final table HTML:", productTableBody.html()); // Debugging
//}
//populateProductTable([
//    { ProductName: "Chocolate 1X", Quantity: 1, Unit: "Kg" },
//    { ProductName: "Sugar", Quantity: 5, Unit: "Kg" }
//]);





// Function to remove a row
function removeRow(button) {
    $(button).closest("tr").remove();
}



// Function to Populate Product Table with Last Order Products
function populateProductTable(products) {
    var tableBody = $('#productTable tbody'); // Adjust table ID as needed
    tableBody.empty(); // Clear previous entries

    products.forEach(function (product, index) {
        var row = `<tr>
            <td>${product.ProductName}</td>
            <td>${product.Quantity}</td>
            <td>${product.PurchasePrice}</td>
            <td><input type="text" name="receivedQuantity" value="${product.Quantity}" /></td>
        </tr>`;
        tableBody.append(row);
    });
}


var productName = "";
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
            pfound = 0;
            $('#selectedProducts > tbody  > tr').each(function () {

                if ($(this).find("[id^='idn']").val() == ui.item[0]) {
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

                this.value = (ui.item ? ui.item[1] : '');
                productName = this.value;
                //if ($('#isPack' + clickedIdNum).val() == "true") {//false=piece true=PerPack
                $('#name' + clickedIdNum).val(ui.item ? ui.item[1] : '');
                $('#perPack' + clickedIdNum).val(ui.item ? ui.item[4] : '');
                //}

                $('#purchasePrice' + clickedIdNum).val(ui.item ? ui.item[2] : '');
                $('#quantity' + clickedIdNum).val(ui.item ? 1 : '');
                $('#idn' + clickedIdNum).val(ui.item ? ui.item[0] : '');
                $('#unit' + clickedIdNum).val(ui.item ? ui.item[5] : '');
                //document.getElementById(clickedTextboxId).focus();
                update_itemTotal();
                //FetchProductRentStatus();
                return false;
            }
        }
    });


    //alert("yes");
}


$(function () {
    //OnTypeName('#name0');
    //alert('#' + clickedTextboxId);


    ConfigDialogueCreateSupplier();

});


$(document).ready(function () {

    //$(this).closest('form').find("input[type=text], textarea").val("");
    if (IsReturn == 'true') {
        $('#saleOrder').hide();
        $('#saleReturn').show();
    }
    else {
        $('#saleReturn').hide();
        $('#saleOrder').show();
    }
    

    //alert('iam ready');
    document.getElementById('supplier').focus();

    //$('#name').tooltip('show')

    //$('[data-toggle="tooltip"]').tooltip();
    var actions = $("table td:last-child").html();
    // Append table with add row form on add new button click

    //$('#addNewRow').keydown(function (event) {
    //    //alert('keydown');
    //    if (event.keyCode == 13) {
    //        $('#addNewRow').trigger('click');
    //    }
    //});
    //$("#addNewRow").click(function () {
    //    txtSerialNum += 1;

    //    var index = $("table tbody tr:last-child").index();

    //    var row = '<tr>' +
    //        '<td id="SNo' + txtSerialNum + '">' + $('#selectedProducts tr').length + '</td>' +
    //        '<td style="display:none;"><input type="hidden" name="PurchaseReceiverOrderDetail.Index" value="' + txtSerialNum + '" /></td>' +
    //        '<td style="display:none;"><input type="hidden" name="PurchaseOrderDetail[' + txtSerialNum + '].ProductId" id="productId' + txtSerialNum + '"></td>' + // Hidden Product ID field
    //        '<td><input type="text" class="form-control" autocomplete="off" name="PurchaseOrderDetail[' + txtSerialNum + '].QtyReceived" id="qtyreceived' + txtSerialNum + '"></td>' +
    //        '<td><input type="datetime-local" class="form-control" autocomplete="off" name="PurchaseOrderDetail[' + txtSerialNum + '].PurchasingDate" id="purchasingdate' + txtSerialNum + '"></td>' +
    //        '<td><input type="datetime-local" class="form-control" autocomplete="off" name="PurchaseOrderDetail[' + txtSerialNum + '].ExpiryDate" id="expirydate' + txtSerialNum + '"></td>' +
    //        '<td><button type="button" id="delete' + txtSerialNum + '" class="delete btn btn-default add-new"> <a class="delete" title="Delete" data-toggle="tooltip"><i class="material-icons">&#xE872;</i></a></button></td>' +
    //        '</tr>';

    //    $("#selectedProducts").append(row);

    //    // Focus on Quantity Received input
    //    document.getElementById('qtyreceived' + txtSerialNum).focus();

    //    // Event Listeners for Auto-Linking Product ID
    //    $("#qtyreceived" + txtSerialNum + ", #purchasingdate" + txtSerialNum + ", #expirydate" + txtSerialNum).on("change", function () {
    //        fetchProductId(txtSerialNum);
    //    });

    //    TriggerBodyEvents();
    //});

    //// Function to Fetch Product ID
    //function fetchProductId(serialNum) {
    //    alert(productId)
    //    var productId = populateProductTable(); // Replace with actual function
    //    $("#productId" + serialNum).val(productId);
    //}

    //$("#addNewRow").click(function (e) {
    //    //alert('click');
    //    //var key = e.which;
    //    //if (key !== 13)  // the enter key code
    //    //{

    //    //    return false;
    //    //}

    //    //$(this).attr("disabled", "disabled");
    //    txtSerialNum += 1;
    //    //alert(txtSerialNum)
    //    var index = $("table tbody tr:last-child").index();
    //    //var rowCount = 
    //    var row = '<tr>' +
    //        '<td id="SNo' + txtSerialNum + '">' + $('#selectedProducts tr').length + '</td>' +
    //        '<td style="display:none;"><input type="hidden" name="PurchaseReciverOrderDetail.Index" value="' + txtSerialNum + '" /></td>' +
    //        '<td style="display:none;"><input type="hidden" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].ProductId" id="productId' + txtSerialNum + '"></td>' + // Hidden Product ID field
    //        '<td><input type="text" class="form-control" autocomplete="off" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].QtyReceived" id="qtyreceived' + txtSerialNum + '"></td>' +
    //        '<td><input type="datetime-local" class="form-control" autocomplete="off" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].PurchasingDate" id="purchasingdate' + txtSerialNum + '"></td>' +
    //        '<td><input type="datetime-local" class="form-control" autocomplete="off" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].ExpiryDate" id="expirydate' + txtSerialNum + '"></td>' +
    //        '<td><button type="button" id="delete' + txtSerialNum + '" class="delete btn btn-default add-new"> <a class="delete" title="Delete" data-toggle="tooltip"><i class="material-icons">&#xE872;</i></a></button></td>' +
    //        '</tr>';

    //    //alert(row);
    //    $("#selectedProducts").append(row);
    //    //alert(txtSerialNum)

    //    $("table tbody tr").eq(index + 1).find(".add, .edit").toggle();
    //    //$('[data-toggle="tooltip"]').tooltip();

    //    document.getElementById('name' + txtSerialNum).focus();
    //    TriggerBodyEvents();

    //});
    //$("#addNewRow").click(function (e) {
    //    txtSerialNum += 1;

    //    // Get ONLY the previous ProductId (ignore QtyReceived)
    //    var previousProductId = $("#selectedProducts tbody tr:last").find("input[name$='.ProductId']").val() || "";

    //    // Create a new row with EMPTY QtyReceived
    //    var row = '<tr>' +
    //        '<td>' + ($("#selectedProducts tbody tr").length + 1) + '</td>' +
    //        '<td style="display:none;"><input type="hidden" name="PurchaseReciverOrderDetail.Index" value="' + txtSerialNum + '" /></td>' +
    //        '<td style="display:none;"><input type="hidden" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].ProductId" value="' + previousProductId + '"></td>' +
    //        '<td><input type="text" class="form-control qty-input" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].QtyReceived" value="" autocomplete="off"></td>' +
    //        '<td><input type="datetime-local" class="form-control" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].PurchasingDate"></td>' +
    //        '<td><input type="datetime-local" class="form-control" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].ExpiryDate"></td>' +
    //        '<td><button type="button" class="delete-row btn btn-default"><i class="material-icons">&#xE872;</i></button></td>' +
    //        '</tr>';

    //    $("#selectedProducts tbody").append(row);

    //    // FORCE-disable autofill (works in all browsers)
    //    $(".qty-input").attr("autocomplete", "new-password");

    //    // Focus on the new input
    //    $("#selectedProducts tbody tr:last .qty-input").focus();
    //});

    $('#addNewRow').keydown(function (event) {
        //alert('keydown');
        if (event.keyCode == 13) {
            $('#addNewRow').trigger('click');
        }
    });

    //$("#addNewRow").click(function (e) {
    //    //alert('click');
    //    //var key = e.which;
    //    //if (key !== 13)  // the enter key code
    //    //{

    //    //    return false;
    //    //}

    //    //$(this).attr("disabled", "disabled");
    //    txtSerialNum += 1;
    //    //alert(txtSerialNum)
    //    var index = $("table tbody tr:last-child").index();
    //    var previousProductId = $("#selectedProducts tbody tr:last").find("input[name$='.ProductId']").val() || "";
    //    //var rowCount = 
    //    var row = '<tr>' +
    //        '<td id="SNo' + txtSerialNum + '">' + $('#selectedProducts tr').length + '</td>' +
    //        '<td style="display:none;"><input type="hidden" name="PurchaseReciverOrderDetail.Index" value="' + txtSerialNum + '" /></td>' +
    //        /*'<td style="display:none;"><input type="text" readonly class="form-control classBGcolor" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].ProductId" id="idn' + txtSerialNum + '"></td>' +*/
    //        /*'<td style="display:none;"><input type="hidden" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].ProductId" value="' + previousProductId + '"></td>' +*/
    //        '<td><input type="text" class="form-control qty-input" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].QtyReceived" value="" autocomplete="off"></td>' +
    //        '<td><input type="datetime-local" class="form-control" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].PurchasingDate"></td>' +
    //        '<td><input type="datetime-local" class="form-control" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].ExpiryDate"></td>' +
    //        '<td><button type="button" id="delete' + txtSerialNum + '" class="delete btn btn-default add-new"> <a class="delete" title="Delete" data-toggle="tooltip"><i class="material-icons">&#xE872;</i></a></button></td>' +
    //        '</tr>';

    //    //alert(row);
    //    $("#selectedProducts").append(row);
    //    //alert(txtSerialNum)

    //    $("table tbody tr").eq(index + 1).find(".add, .edit").toggle();
    //    //$('[data-toggle="tooltip"]').tooltip();

    //    document.getElementById('name' + txtSerialNum).focus();
    //    TriggerBodyEvents();

    //});

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
        /*var previousProductId = $("#selectedProducts tbody tr:last").find("input[name$='.ProductId']").val() || "";*/
        //var rowCount = 
        var row = '<tr>' +
            '<td id="SNo' + txtSerialNum + '">' + $('#selectedProducts tr').length + '</td>' +
            '<td style="display:none;"><input type="hidden" name="PurchaseReciverOrderDetail.Index" value="' + txtSerialNum + '" /></td>' +
            '<td style="display:none;"><input type="text" readonly class="form-control classBGcolor" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].ProductId" id="idn' + txtSerialNum + '"></td>' +
            '<td><input type="text" class="form-control" autocomplete="off" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].QtyReceived" id="qtyreceived' + txtSerialNum + '"></td>' +
            '<td><input type="datetime-local" class="form-control" autocomplete="off" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].PurchasingDate" id="purchasingdate' + txtSerialNum + '"></td>' +
            '<td><input type="datetime-local" class="form-control" autocomplete="off" name="PurchaseReciverOrderDetail[' + txtSerialNum + '].ExpiryDate" id="expirydate' + txtSerialNum + '"></td>' +
            '<td><button type="button" id="delete' + txtSerialNum + '" class="delete btn btn-default add-new"> <a class="delete" title="Delete" data-toggle="tooltip"><i class="material-icons">&#xE872;</i></a></button></td>' +
            '</tr>';

        //alert(row);
        $("#selectedProducts").append(row);
        //alert(txtSerialNum)

        $("table tbody tr").eq(index + 1).find(".add, .edit").toggle();
        //$('[data-toggle="tooltip"]').tooltip();
        $(document).on("click", ".delete-row", function () {
            $(this).closest("tr").remove(); // Remove the row correctly
        });
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

    $('#CreatePO').keydown(function (event) {
        if (event.keyCode == 13) {
            $('#CreatePO').trigger('click');
        }
    });
    $('#CreatePO').click(function () {

        if ($('#idnSupplier').val() == "") {
            alert('Supplier not found. Please select supplier from list or add new');
            return false;

        }

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
        //    $('#productsTable tr').each(function () {
        //        tblProductId1 = $(this).find(".ProductId").html();
        //        if (typeof tblProductId1 != 'undefined') {
        //            if (EnteredProductId1 == tblProductId1.trim()) {
        //                //alert('abc');
        //                tblProductStock1 = $(this).find(".stock").html().trim();
        //            }
        //        }
        //    });
        //    if (Number(EnteredQty1) > Number(tblProductStock1) && saleType1 == "false") {
        //        alert("Item # " + idx1 + " available stock is " + tblProductStock1);
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
        var wentRight = 1;
        var InvalidproductName= ' ';
        var idx = 0;



        //if (isNaN($('#total').val())) {
        //    alert('Total is not valid');
        //    return false;
        //}
        //
        //if (isNaN($('#balance').val())) {
        //    alert('Balance is not valid');
        //    return false;
        //}

        $('#selectedProduct > tbody  > tr').each(function () {
            idx += 1;
            var price = $(this).find("[id^='purchasePrice']").val();
            InvalidproductName = $(this).find("[id^='name']").val();
            //alert(price);
            if (!price) {
                //alert(price + " returning");
                wentRight = 0;
                return false;

            }
        });

        //if (wentRight == 0) {
        //    //alert("item # " + idx + " " + InvalidproductName + " is not a valid product name. Please select valid product from product list");
        //    alert("(Item # " + idx + ") " + InvalidproductName + " Please select appropriate product name from list");
        //    return false;
        //}

        if (checkAvaiableStock() == false) return false;

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
        //if ($('#ReturnItemsTotal').val().trim() == "") {
        //    $('#ReturnItemsTotal').val(0);
        //}

        //$("#CreatePO").attr("disabled", true);
        $('form').preventDoubleSubmission();

    });

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

    //$(document).on("click", "OpenNewSuppForm", function () {
    //    $("#dialog-CreateSupplier").dialog("open");
    //});

    $('#OpenNewSuppForm').click(function () {
        
        $("#dialog-CreateSupplier").dialog("open");
    });

    $('#btnCreateNewSupp').click(function () {

        $("#dialog-CreateSupplier").dialog("close");
        //$('#idnSupplier').val(SupplierId);
        var contents = $("#NewSupplierId").val();
        $("#idnSupplier").val(contents);

        contents = $("#NewSupplierName").val();

        $("#supplier").val(contents);

        contents = $("#NewSupplierAddress").val();
        $("#supplierAddress").val(contents);

        $("#PreviousBalance").val(0.00);
        update_itemTotal();
        //alert(contents);
    });

    //$('[id^="saleType"]').change(function () {
    //    update_itemTotal();
    //});

   
    
});


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
        $('#purchasePrice' + clickedIdNum).val(result[2]);
        $('#quantity' + clickedIdNum).val(1);
        $('#idn' + clickedIdNum).val(result[0]);
        $('#PerPack' + clickedIdNum).val(result[4]);
        //document.getElementById(clickedTextboxId).focus();
        update_itemTotal();
        $('#addNewRow').trigger('click');
    }

}

function TriggerBodyEvents() {
    //alert('#name' + txtSerialNum);
    OnTypeName('#name' + txtSerialNum);
    var _keybuffer = "";
    $('#name' + txtSerialNum).on("keyup", function (e) {
        /*alert('#name' + txtSerialNum);*/
        //if (e.keyCode === 13)
        //{
        //    alert('enter');
        //}
        var code = e.keyCode || e.which;
        //alert(code);
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
    $('#purchasePrice' + txtSerialNum).keyup(function () {
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
            $("#CreatePO").html("Pay " + paid);
        }
        else {
            $("#CreatePO").html("Return " + paid);
        }
    });

}
function ConfigDialogueCreateSupplier() {
    //alert("create Supplier configured");
    $("#dialog-CreateSupplier").dialog({
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
            $('#dialog-CreateSupplier').css('overflow', 'hidden'); //this line does the actual hiding
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
        //alert(saleType1);
        if (Number(EnteredQty1) > Number(tblProductStock1) && saleType1 == "true") {
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
    
    var ItemsTotal = 0;
    
    var orderQty = 0;
    var orderQtyPiece = 0;
    var returnQty = 0;
    var returnQtyPiece = 0;
    var SN = 0;
    $("#OrderTotal").val('Order Total(' + parseInt(orderQty) + ')');
    
    $('#selectedProducts > tbody  > tr').each(function () {
        //if (IsReturn == 'true') {//if (IsReturn == 'True') { c# model uses 'T'rue and java script user 't'rue
        if (IsReturn == 'false') {
            //$('#name').prop('selectedIndex', 0);

            $(this).find("[id^='saleType']").prop('selectedIndex', 0);
            //alert('aaa');
        }
        else {

            $(this).find("[id^='saleType']").prop('selectedIndex', 1);
            //alert(IsOrder);
        }

        SN += 1;
        $(this).find("[id^='SNo']").text(SN);

        var qty = 0;
        var price = 0;
        var perPack = 0;
        //alert(IsReturn);
        //alert($(this).find("[id^='saleType']").val());
        //if ($(this).find("[id^='saleType']").val() == "false") {
        if (IsReturn == 'false') {
            qty = $(this).find("[id^='quantity']").val();
            if (!(qty)) { qty = 0; }
            price = $(this).find("[id^='purchasePrice']").val();
            
            var itemAmount = (qty * price);
            if ($(this).find("[id^='isPack']").val() == "true") {//false=item true=PerPack
                perPack = $(this).find("[id^='perPack']").val();
                if (perPack == 0) perPack = 1;
                itemAmount = itemAmount * perPack;
                orderQty += parseInt(qty);
            }
            else {
                orderQtyPiece += parseInt(qty);
            }
            ItemsTotal += itemAmount;
            $(this).find("[id^='itemTotal']").val(itemAmount.toFixed(2));
        } else {
            //alert($('#saleType').val());
            qty = $(this).find("[id^='quantity']").val();
            if (!(qty)) { qty = 0; }
            price = $(this).find("[id^='purchasePrice']").val();
            
            var ItemAmount = (qty * price);
            if ($(this).find("[id^='isPack']").val() == "true") {//false=item true=PerPack
                perPack = $(this).find("[id^='perPack']").val();
                if (perPack == 0) perPack = 1;
                ItemAmount = ItemAmount * perPack;
                orderQty += parseInt(qty);
                //alert(ItemAmount);
            } else {
                orderQtyPiece += parseInt(qty);
            }
            ItemsTotal += ItemAmount;
            $(this).find("[id^='itemTotal']").val(ItemAmount.toFixed(2));


        }

        //$("#OrderTotal").val('Order Total(' + parseInt(orderQty) + ' pack, ' + parseInt(orderQtyPiece) + ' piece)');
        $("#OrderTotal").val('Order Total(' + parseInt(orderQty) + ')');
        
    });

    //$('#OpenNewSuppForm').click(function () {
    //    $("#dialog-CreateSupplier").dialog("open");
    //});

    //$('[id^="quantity"]').change(function () {
    //    alert("");
    //});

    //jQuery('[id^="quantity"]').on('change input propertychange paste', function () {
    //    alert("c");
    //});

    /////////////////////////////////
    //$('[id^="quantity"]').blur(function () {
    //    //var inputObj = this;
    //    var EnteredQty = 0;
    //    var EnteredProductId = 0;
    //    var tblProductStock = 0;
    //    var tblProductId = 0;
    //    var saleType = false;
    //    EnteredQty = this.value.trim();
    //    //alert(this.value.trim());
    //    EnteredProductId = $(this).closest("tr").find("[id^='idn']").val().trim();
    //    saleType = $(this).closest("tr").find("[id^='saleType']").val().trim();


    //    $('#productsTable tr').each(function () {
    //        tblProductId = $(this).find(".ProductId").html();

    //        if (typeof tblProductId != 'undefined') {
    //            if (EnteredProductId == tblProductId.trim()) {
    //                //alert('abc');
    //                tblProductStock = $(this).find(".stock").html().trim();
    //                //alert(tblProductStock.trim());
    //                //if (EnteredQty.trim() > tblProductStock.trim()) {
    //                //    alert("available stock is " + tblProductStock);
    //                //    return false; 
    //                //}
    //                //return false;
    //            }
    //        }
    //    });
    //    //alert(EnteredQty.trim());
    //    //alert(tblProductStock.trim());
    //    if (Number(EnteredQty) > Number(tblProductStock) && saleType == "false") {

    //        alert("available stock is " + tblProductStock);
    //        $(this).closest("tr").find("[id^='quantity']").val(EnteredQty.trim());
    //        //return false;

    //    }

    //    //$('#productsTable .ProductIdd').each(function () {
    //    //    alert($(this).html());
    //    //});

    //});
    /////////////////////////////////////




    //$('[id^="saleType"]').change(function () {
    //    //var end = this.value;
    //    //var firstDropVal = $('#saleType').val();
    //    update_itemTotal();
    //});

    //$('[id^="quantity"]').change(function () {
    //    //var end = this.value;
    //    //var firstDropVal = $('#saleType').val();

    //    update_itemTotal();
    //});


    //jQuery('[id^="quantity"]').on('input propertychange paste', function () {
    //    alert("qunatity was entered");
    //});

    //if (ItemsTotal > 0 || ReturnsTotal > 0) {
    //    $('#dvCalculations').show();
    //} else {
    //    $('#dvCalculations').hide();
    //}

    $('#ItemsTotal').val(ItemsTotal.toFixed(2));//for input element
    

    var discount = $('#discount').val();
    var prevBal = parseFloat($("#PreviousBalance").val());
    var subtotal = ItemsTotal - discount;
    var total = 0;
    if (IsReturn == 'false') {
        total = subtotal + prevBal;
        $('#pb3').val('Prev. Balance');
    }
    else {
        total = subtotal - prevBal;
        $('#pb3').val('Prev. Balance                             -');
    }

    //total += $("#PreviousBalance").val();
    //alert(subtotal);
    $('#subtotal').val(subtotal.toFixed(2));
    $('#total').val(total.toFixed(2));
    $('#paid').val(total.toFixed(2));
    _total = total;
    //alert(ItemsTotal + ", " + discount + ", " + ReturnsTotal + ", " + prevBal);
    var paid = $('#paid').val();
    var balance = _total - paid;
    $('#balance').val(balance.toFixed(2));
    if (IsReturn == 'false') {
        $("#CreatePO").html("Pay " + paid);
    }
    else {
        $("#CreatePO").html("Return " + paid);
    }

       
}

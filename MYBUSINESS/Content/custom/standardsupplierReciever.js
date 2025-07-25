﻿var productColumns = [{ name: 'Id', minWidth: '100px' }, { name: 'Product', minWidth: '320px' }, { name: 'Purch. Price', minWidth: '100px' }, { name: 'Stock', minWidth: '70px' }, { name: 'PerPack', minWidth: '70px' }, { name: 'Unit', minWidth: '70px' }];
//var products = []; //[['Ciplet', '10', '60'], ['Gaviscon', '85', '12'], ['Surficol', '110', '8']];
var products = new Array();

var supplierColumns = [{ name: 'Id', minWidth: '100px' }, { name: 'Name', minWidth: '320px' }, { name: 'Address', minWidth: '200px' }, { name: 'Balance', minWidth: '70px' }];
//var products = []; //[['Ciplet', '10', '60'], ['Gaviscon', '85', '12'], ['Surficol', '110', '8']];
var suppliers = new Array();
var productsBarcodes = new Array();
//var focusedBtnId = "";
//var focusedBtnSno = "";
var txtSerialNum = 0;
var clickedTextboxId = "name0";
var clickedIdNum = "";
var x, y;
var _total = 0;
var IsReturn = "false";
function OnTypeSupplierName(param) {
    $(param).mcautocomplete({
        showHeader: true,
        columns: supplierColumns,
        source: suppliers,
        select: function (event, ui) {
            this.value = (ui.item ? ui.item[1] : '');
            //productName = this.value;
            $('#idnSupplier').val(ui.item ? ui.item[0] : '');
            $('#supplierAddress').val(ui.item ? ui.item[2] : '');
            $('#PreviousBalance').val(ui.item ? ui.item[3] : '');
            update_itemTotal();
            document.getElementById('name' + txtSerialNum).focus();
            return false;
        }

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
    // Global variables to track quantities
    let orderedQuantity = 0;
    let previouslyReceived = 0;
    let currentReceivedTotal = 0;

    // Initialize quantities on page load
    function initializeQuantities() {
        // Get ordered quantity from the first table
        orderedQuantity = parseFloat($('.ordered-quantity').first().val()) || 0;

        // Reset counters
        previouslyReceived = 0;
        currentReceivedTotal = 0;

        // Calculate quantities from all rows
        $('#selectedProducts tbody tr').each(function (index) {
            const receivedInput = $(this).find('.received-qty');
            if (receivedInput.length) {
                const qty = parseFloat(receivedInput.val()) || 0;

                // For edit view, first row is typically the previously received quantity
                if (index === 0) {
                    previouslyReceived = qty;
                } else {
                    currentReceivedTotal += qty;
                }
            }
        });

        console.log(`Initialized - Ordered: ${orderedQuantity}, Previously Received: ${previouslyReceived}, Current Editable: ${currentReceivedTotal}`);
    }

    // Validate a single quantity input
    function validateQuantityInput(input) {
        const totalWithCurrent = getTotalReceivedIncludingCurrentInput(input);

        if (totalWithCurrent > orderedQuantity) {
            const allowedRemaining = orderedQuantity - previouslyReceived - calculateCurrentReceivedExcept(input);
            alert(`Total received quantity (${totalWithCurrent}) exceeds ordered quantity (${orderedQuantity}). You can only receive up to ${allowedRemaining}.`);

            // Reset the field to allowed max or blank
            $(input).val(allowedRemaining > 0 ? allowedRemaining : '');
            $(input).focus();
            return false;
        }

        return true;
    }


    // Calculate current received quantities except the specified input
    function calculateCurrentReceivedExcept(excludeInput) {
        let total = 0;
        $('#selectedProducts tbody tr:not(:first) .received-qty').each(function () {
            if (!excludeInput || this !== excludeInput[0]) {
                total += parseFloat($(this).val()) || 0;
            }
        });
        return total;
    }

    // Validate all quantities before save
    function validateBeforeSave() {
        let total = previouslyReceived;

        $('#selectedProducts .received-qty:not([readonly])').each(function () {
            total += parseFloat($(this).val()) || 0;
        });

        if (total > orderedQuantity) {
            const overBy = total - orderedQuantity;
            alert(`Total received quantity exceeds ordered quantity by ${overBy}. Please adjust.`);
            return false;
        }
        return true;
    }


    // Add new row functionality
    $("#addNewRow").click(function () {
        if (!validateBeforeSave()) {
            return false;
        }

        const newRowIndex = $("#selectedProducts tbody tr").length;
        const newRow = `
            <tr>
                <td>${newRowIndex + 1}</td>
                <td style="display:none;">
                    <input type="hidden" name="SPackgingReceiver.Index" value="${newRowIndex}" />
                </td>
                <td style="display:none;">
                    <input type="text" readonly class="form-control" name="SPDReceiver[${newRowIndex}].ProductId">
                </td>
                <td>
                    <input type="number" step="0.001" class="form-control received-qty" 
                           name="SPDReceiver[${newRowIndex}].QtyReceived" min="0">
                </td>
                <td>
                    <input type="datetime-local" class="form-control" 
                           name="SPDReceiver[${newRowIndex}].PurchasingDate">
                </td>
                <td>
                    <input type="datetime-local" class="form-control" 
                           name="SPDReceiver[${newRowIndex}].ExpiryDate">
                </td>
                <td>
                    <button type="button" class="delete btn btn-default add-new">
                    <a class="delete" title="Delete" data-toggle="tooltip">
                        <i class="material-icons">&#xE872;</i>
                     </a>
                    </button>
                </td>
            </tr>`;

        $("#selectedProducts tbody").append(newRow);

        // Set up event handlers for new row
        $(`#selectedProducts tr:last .received-qty`).on('change', function () {
            validateQuantityInput($(this));
        });
    });

    // Delete row functionality
    $(document).on('click', '.delete', function () {
        // Don't allow deleting the first row (previously received)
        if ($(this).closest('tr').index() === 0) {
            alert("Cannot delete the previously received quantity row");
            return;
        }
        $(this).closest('tr').remove();
    });

    // Form submission handler
    $("#POEdit").on('submit', function (e) {
        if (!validateBeforeSave()) {
            e.preventDefault();
            return false;
        }
        return true;
    });

    // Initialize quantities and event handlers
    initializeQuantities();
    $('.received-qty').on('change', function () {
        validateQuantityInput($(this));
    });
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

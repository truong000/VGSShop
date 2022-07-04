$(function () {
    $(".add-to-cart1").click(function () {
        var productid = $(this).attr("data-productid");
        var soLuong = 1;
        $.ajax({
            url: '/api/cart/add',
            type: "POST",
            dataType: "JSON",
            data: {
                productID: productid,
                amount: soLuong
            },
            success: function (response) {
                if (response.result == 'Redirect') {
                    window.location = response.url;
                }
                else {
                    loadHeaderCart(); //Add Product success
                    location.reload();
                }
                console.log(response); // log to the console to see whether it worked
            },
            error: function (error) {
                alert("There was an error posting the data to the server: " + error.responseText);
            }
        });
    });
});
function loadHeaderCart() {
    $('#miniCart').load("/AjaxContent/HeaderCart");
    $('#numberCart').load("/AjaxContent/NumberCart");
}

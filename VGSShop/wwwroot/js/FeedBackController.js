var product = {
    init: function () {
        product.registerEvents();
    },
    registerEvents: function () {
        $('.btn-active').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/Contact/ChangeStatus",
                data: { id: id },
                dataType: "json",
                type: "POST",
                success: function (response) {
                    console.log(response);
                    if (response.status == true) {
                        btn.text('Kích hoạt');
                    }
                    else {
                        btn.text('Khoá');
                    }
                }
            });
        });
        $('#btnSend').off('click').on('click', function () {
            var name = $('#txtName').val();
            var mobile = $('#txtMobile').val();
            var address = $('#txtAddress').val();
            var email = $('#txtEmail').val();
            var content = $('#txtContent').val();

            $.ajax({
                url: '/FeedBack/Send',
                type: 'POST',
                dataType: 'json',
                data: {
                    name: name,
                    mobile: mobile,
                    address: address,
                    email: email,
                    content: content
                },
                success: function (res) {
                    if (res.status == true) {
                        window.alert('Gửi thành công');
                        contact.resetForm();
                    }
                }
            });
        });
    },
    resetForm: function () {
        $('#txtName').val('');
        $('#txtMobile').val('');
        $('#txtAddress').val('');
        $('#txtEmail').val('');
        $('#txtContent').val('');
    }
}
product.init();
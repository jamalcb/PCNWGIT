$(document).ready(function () {
    BindFaq(0);
});
function createDataTables() {
    setTimeout(function () {
        $('.fTables').datatables();
    }, 1000);
}
function reinitialLizedDataTables() {
    $('.fTables').dataTable().fnClearTable();
    $('.fTables').dataTable().fnDestroy();
}
function OpenPrintModel() {
    $("#prev-btn").click();
    $("#frmPrintForm").trigger('reset');
    $('#frmPrintForm').find("input[type=text], textarea").val('');
    $('span.loginError').html('');
    $(".m-modal").modal('show');
}
function BindFaq(isReinitialized) {
    if (isReinitialized == 1)
        reinitialLizedDataTables();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/GlobalMaster/GetFaqList',
        data: {},
        async: false,
        success: function (response) {
            var rows = '';
            var tdrow = '';
            $.each(response.data, function (index, item) {
                rows += '<tr><td style="font-weight: bold;">' + item.Question + '</td><td class="text-justify">' + item.Answer + '</td><td class="">' + (item.IsActive == true ? "Active" : "InActive") + '</td><td><span title="Edit" class="btn btn-primary icon-edit" onclick="EditFaq(this, ' + item.Id + ')" ><i class="fas fa-pencil-alt" aria-hidden="true"></i></span></td></tr>';
            });
            $('#tblFaq').html(rows);
            createDataTables();
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }
    });
}
function SaveFaq() {
    var ques = $('#Question').val();
    $('#Question').next('span').html('');
    if (ques == undefined || ques == '' || ques == null) {
        $('#Question').next('span').html('Please enter your question.');
        return false;
    }
    //var ans = $('#Answer').val();
    var ans = CKEDITOR.instances.Answer.getData();
    $('#Answer').next('span').html('');
    if (ans == undefined || ans == '' || ans == null) {
        $('#Answer').next('span').html('Please enter your answer.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.Id = $('#Id').val();
    model.Question = $('#Question').val();
    model.Answer = ans;
    model.IsActive = $('#IsActive').is(':checked') ? true : false;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/GlobalMaster/SaveFaq/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                console.log(response);
                BindFaq(1);
                alert(response.statusMessage);
            }
            else {
                $('#message').fadeIn();
                $('#message').text(response.statusMessage).fadeOut(5000);

            }
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}
function EditFaq(Ctrl, Id) {
    OpenPrintModel();
    $('#Id').val(Id);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#Question').val(rows.find('td:eq(0)').text());
    console.log(rows.find('td:eq(1)').text());
    $('#Answer').val(rows.find('td:eq(1)').text());
    var html = rows.find('td:eq(1)').text();
    CKEDITOR.instances['Answer'].setData(html)
    var activeText = rows.find('td:eq(2)').text();
    if (activeText == "Active")
        $('#IsActive').prop('checked', true);
    else
        $('#IsActive').prop('checked', false);
}


    
function OpenEntityModel() {
    $("#EntityForm").trigger('reset');
    $('#EntityForm').find("input[type=text], textarea").val('');
    $('span.loginError').html('');
    $("#Headertext").text('Add New Entity');
    $(".m-modal").modal('show');
}
function SaveEntityType() {
    var Etype = $('#EntityType').val();
    $('#EntityType').next('span').html('');
    if (Etype == undefined || Etype == '' || Etype == null) {
        $('#EntityType').next('span').html('Please enter your entity type.');
        return false;
    }
    var model = {};
    $('#message').val('');
    model.EntityID = $('#EntityID').val();
    model.EntityType = $('#EntityType').val();
    model.IsActive = $('#IsActive').is(':checked') ? true : false;
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: '/StaffAccount/SaveEntityType/',
        data: { 'model': model },
        async: false,
        success: function (response) {
            if (response.success) {
                alert(response.statusMessage);
                $(".m-modal").modal('hide');
                var pathname = 'Entity';
                window.location.href = "/StaffAccount/MemberManagement?returnUrl=" + pathname;
            }
            else {
                $('#EntityType').next('span').html(response.statusMessage);
            }
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });
}
function EditEntityType(Ctrl, EntityID) {
    OpenEntityModel();
    $('#EntityID').val(EntityID);
    var rows = $(Ctrl).parent('td').parent('tr');
    $('#EntityType').val(rows.find('td:eq(0)').text());
    $('#IsActive').prop('checked', true);
    $("#Headertext").text('Update Entity');
}

function DeleteEntityType(EntityID,ctrl) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/StaffAccount/DeleteEntityType/',
            data: { 'EntityID': EntityID },
            async: false,
            success: function (response) {
                if (response.success) {
                    alert(response.statusMessage);
                    $(ctrl).parents("tr").remove();
                }
            },
            error: function (response) {
            },
            failure: function (response) {
            }
        });
    }
    return false;
}

function OtherContactTab(ctrl) {
    var form = $(ctrl).parent();
    var ActiveElement = ctrl.nextElementSibling;
    ActiveElement.value = 'about';
    form.submit();
}

function DeleteMember(id,ctrl) {
    if (confirm("Are you sure you want to delete ?")) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: '/StaffAccount/DeleteMember/',
            data: { 'id': id },
            async: false,
            success: function (response) {
                if (response.success) {
                    alert(response.statusMessage);
                    window.location.reload();
                }
            },
            error: function (response) {
            },
            failure: function (response) {
            }
        });
    }
    return false;
}


function OtherContactDelete(asp_action,id,ctrl) {
    if (asp_action == "MemberProfile") {
        if (confirm("Are you sure you want to delete permanently?")) {
            $.ajax({
                type: "POST",
                dataType: 'json',
                url: '/StaffAccount/DeleteInactiveMember/',
                data: { 'id': id },
                async: false,
                success: function (response) {
                    if (response.success) {
                        alert(response.statusMessage);
                        $(ctrl).parents("tr").remove();
                    }
                },
                error: function (response) {
                },
                failure: function (response) {
                }
            });
        }
        return false;
    }
    else if (asp_action == "ContractorProfile") {
        if (confirm("Are you sure you want to delete ?")) {
            $.ajax({
                type: "POST",
                dataType: 'json',
                url: '/StaffAccount/DeleteContractor/',
                data: { 'id': id },
                async: false,
                success: function (response) {
                    if (response.success) {
                        alert(response.statusMessage);
                        $(ctrl).parents("tr").remove();
                    }
                },
                error: function (response) {
                },
                failure: function (response) {
                }
            });
        }
        return false;
    }
    else if (asp_action == "ArchitectProfile") {
        if (confirm("Are you sure you want to delete ?")) {
            $.ajax({
                type: "POST",
                dataType: 'json',
                url: '/StaffAccount/DeleteArchitect/',
                data: { 'id': id },
                async: false,
                success: function (response) {
                    if (response.success) {
                        alert(response.statusMessage);
                        $(ctrl).parents("tr").remove();
                    }
                },
                error: function (response) {
                },
                failure: function (response) {
                }
            });
        }
        return false;
    }
}
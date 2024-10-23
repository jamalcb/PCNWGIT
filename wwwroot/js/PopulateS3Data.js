function PopulateAddenda() {
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: '/Administration/PopulateAddenda',
        async: false,
        success: function (response) {
            console.log(response);
            if (response.Status == "success")
                alert('Data updated succesfully');
        },
        error: function (response) {
            alert('Something went wrong');
        },
        failure: function (response) {
            alert('Something went wrong');
        }
    });
}

var uri = 'api/members';
var uriGen = 'api/generator';
var idIncrement = 1;

function getAllMembers() {
    // Send an AJAX request
    $('#members').empty();
    $.getJSON(uri)
        .done(function (data) {
            // On success, 'data' contains a list of products.
            $.each(data, function (key, item) {
                // Add a list item for the product.
                $('<li>', { text: formatItem(item) }).appendTo($('#members'));
            });
        });
}

function formatItem(item) {
    return item.Id + ': ' + item.Name;
}

function find() {
    var id = $('#memberId').val();
    var returnData = "";
    $.getJSON(uri + '/' + id)
        .done(function (data) {
            $('#member').text(formatItem(data));
            returnData = formatItem(data);
        })
        .fail(function (jqXHR, textStatus, err) {
            $('#member').text('Error: ' + err);
        });
}

function add(InputName) {
    var sendJsonData = {
        Id: idIncrement,
        Name: InputName
    };

    $.ajax({
        url: uri,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(sendJsonData),
        success: function (data) {
            var ul = document.getElementById("members");
            var li = document.createElement("li");
            li.setAttribute("class", "list-group-item");
            li.appendChild(document.createTextNode(data.Name));
            ul.appendChild(li);
        }
    });
    idIncrement++;
}

function formatSchemeItem(item) {
    return item.text;
}

function generateScheme() {
    $.getJSON(uriGen + '/' + 1)
        .done(function (data) {
            $.each(data.dateList, function (key, item) {
                // Add a list item for the product.
                $('<li>', { text: item }).attr('class','list-group-item').appendTo($('#party'), '</li>');
            });
        })
        .fail(function (jqXHR, textStatus, err) {
            $('#party').text('Error: ' + err);
        });
}


var uri = 'api/members';
var uriGen = 'api/generator';
var idIncrement = 1;

function getAllMembers() {
  // Send an AJAX request
    $.ajax({
      url: uri,
      type: "GET",
      contentType: "application/json",
      success: function(data) {
        $.each(data, function(key, item) {
          var ul = document.getElementById("members");
          var li = document.createElement("li");
          li.setAttribute("class", "list-group-item");
          li.appendChild(document.createTextNode(item.name));
          ul.appendChild(li);
        });
      }
    });
}

function formatItem(item) {
  return item.Id + ': ' + item.Name;
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
    success: function(data) {
      var ul = document.getElementById("members");
      var li = document.createElement("li");
      li.setAttribute("class", "list-group-item");
      li.appendChild(document.createTextNode(data.name));
      ul.appendChild(li);
    }
  });
  idIncrement++;
}

function formatSchemeItem(item) {
  return item.text;
}


function createTable(data){
  var table = document.getElementById("scheduleTable");
  var header = table.createTHead();
  var headerRow = header.insertRow(0);
  headerRow.insertCell(-1).innerHTML = "<b>Name</b>";
  headerRow.insertCell(-1).innerHTML = "<b>Duty</b>";
  headerRow.insertCell(-1).innerHTML = "<b>Starter</b>";
  headerRow.insertCell(-1).innerHTML = "<b>Dinner</b>";
  headerRow.insertCell(-1).innerHTML = "<b>Dessert</b>";
  var body = table.createTBody();
  $.each(data.dateList, function(key, item) {
    var row = body.insertRow(0);
    var cell0 = row.insertCell(-1);
    var cell1 = row.insertCell(-1);
    var cell2 = row.insertCell(-1);
    var cell3 = row.insertCell(-1);
    var cell4 = row.insertCell(-1);
    cell0.innerHTML = item.name;
    cell1.innerHTML = item.duty;
    cell2.innerHTML = item.starter;
    cell3.innerHTML = item.dinner;
    cell4.innerHTML = item.dessert;
  });
}

function generateScheme() {
  $.ajax({
    url: uriGen + '/' + 1,
    type: "GET",
    contentType: "application/json",
    success: function(data) {
      createTable(data);
    }
  });

}

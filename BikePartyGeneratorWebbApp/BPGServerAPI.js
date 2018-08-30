
var uri = 'api/members';
var uriGen = 'api/generator';

function formatNames(item) {
  var formatedNamesString = '';
  if(item === null){
    return formatedNamesString;
  }
  for (var i = 0; i < item.length; i++) {
    if (i === item.length - 1 && item.length > 1) {
      formatedNamesString += ' and ';
    } else if (i !== 0 && item.length > 1) {
      formatedNamesString += ', ';
    }
    formatedNamesString += item[i];
  }
  return formatedNamesString;
}

function loadNavbar(itemId) {
  $('#navbar').load("navbar-template.html", function() {
    // this is runned after header.html has been loaded
    $(itemId).addClass('active');
  });
}

function getAllMembers() {
  $.ajax({
    url: uri,
    type: "GET",
    contentType: "application/json",
    success: function(data) {
      // Gets all members and creates a list on the register page when loaded
      $.each(data, function(key, item) {
        var ul = document.getElementById("members");
        var li = document.createElement("li");
        var span = document.createElement('span');
        span.setAttribute("class", "close");
        span.onclick = function() {
          removeMember(this.parentNode.id);
        };
        span.innerHTML = '&times';

        li.setAttribute("class", "list-group-item");
        li.setAttribute("id", item.Id);
        li.appendChild(span);
        li.appendChild(document.createTextNode(formatNames(item.names)));
        ul.appendChild(li);
      });
    }
  });
}

function registerMember() {
  const InputNamesNodeList = document.getElementsByName("name");
  var nameInput = [];

  $.each(InputNamesNodeList, function(index, value) {
    nameInput.push(value.value);
    value.value = '';
  });

  const associationInput = document.getElementById('associationDropdown');
  const addressInput = document.getElementById('addressInput');
  const phoneInput = document.getElementById('phoneInput');
  add(nameInput, addressInput.value, phoneInput.value, associationInput.selectedIndex);
  addressInput.value = '';
  phoneInput.value = '';
  document.getElementById('nameInput').focus();
}

function add(InputName, InputAddress, InputPhone, associationInput) {
  var sendJsonData = {
    names: InputName,
    address: InputAddress,
    phone: InputPhone,
    association: associationInput
  };

  $.ajax({
    url: uri,
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(sendJsonData),
    success: function(data) {
      //Add this new member to the list on the register page
      var ul = document.getElementById("members");
      var li = document.createElement("li");
      var span = document.createElement('span');
      span.setAttribute("class", "close");
      span.onclick = function() {
        removeMember(this.parentNode.id);
      };
      span.innerHTML = '&times';
      li.setAttribute("class", "list-group-item");
      li.setAttribute("id", data.Id);
      li.appendChild(span);
      li.appendChild(document.createTextNode(formatNames(data.names)));
      ul.appendChild(li);
    }
  });
}

function removeMember(id) {
  $.ajax({
    url: uri + "/RemoveMember/" + id,
    type: "POST",
    contentType: "application/json"
  });
  $("#" + id).remove();
}

function createTable(data) {
  if (data.message !== null) {
    var message = document.getElementById("message");
    message.innerHTML = data.message;
    message.style.display = "block";
  }
  if (data.dateList.length > 0) {
    var table = document.getElementById("scheduleTable");
    $("#scheduleTable tr").remove();
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
      cell0.innerHTML = formatNames(item.names);
      cell1.innerHTML = item.duty;
      cell2.innerHTML = formatNames(item.starter);
      cell3.innerHTML = formatNames(item.dinner);
      cell4.innerHTML = formatNames(item.dessert);
    });
  }
}

function generateScheme() {
  $.ajax({
    url: uriGen,
    type: "GET",
    contentType: "application/json",
    success: function(data) {
      createTable(data);
    }
  });

}

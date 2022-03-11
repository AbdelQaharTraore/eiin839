var stations = []
function retrieveAllContracts() {
  //   var API_KEY = "f5f2574c3604270c64f878e398b181acdaee4a18"
  var targetUrl =
    "https://api.jcdecaux.com/vls/v3/contracts?apiKey=" +
    document.getElementById("apiKey").value;
  var requestType = "GET";

  var caller = new XMLHttpRequest();
  caller.open(requestType, targetUrl, true);
  // The header set below limits the elements we are OK to retrieve from the server.
  caller.setRequestHeader("Accept", "application/json");
  // onload shall contain the function that will be called when the call is finished.
  caller.onload = contractsRetrieved;

  caller.send();
}

function contractsRetrieved() {
  // Let's parse the response:
  var response = JSON.parse(this.responseText);
  for (var i = 0; i < response.length; i++) {
    var contractName = document.createElement("option");
    contractName.setAttribute("value", response[i].name);
    document.getElementById("dal_id").appendChild(contractName);
  }
}
// -----------------------------------------------------------------
function retrieveContractStations() {
  var targetUrl =
    "https://api.jcdecaux.com/vls/v3/stations?contract=" +
    document.getElementById("contractName").value
    + "&apiKey="
    + document.getElementById("apiKey").value;
  var requestType = "GET";

  var caller = new XMLHttpRequest();
  caller.open(requestType, targetUrl, true);
  // The header set below limits the elements we are OK to retrieve from the server.
  caller.setRequestHeader("Accept", "application/json");
  // onload shall contain the function that will be called when the call is finished.
  caller.onload = stationsRetrieved;

  caller.send();
}

function stationsRetrieved() {
  // Let's parse the response:
  var response = JSON.parse(this.responseText);
  for (var i = 0; i < response.length; i++) {
    stations = []
    stations.push(response[i]);
    console.log(response[i].name,
      response[i].position.latitude,
      response[i].position.longitude);
  }
}
// -----------------------------------------------------------------
function getDistanceFrom2GpsCoordinates(lat1, lon1, lat2, lon2) {
  // Radius of the earth in km
  var earthRadius = 6371;
  var dLat = deg2rad(lat2 - lat1);
  var dLon = deg2rad(lon2 - lon1);
  var a =
    Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(deg2rad(lat1)) *
      Math.cos(deg2rad(lat2)) *
      Math.sin(dLon / 2) *
      Math.sin(dLon / 2);
  var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  var d = earthRadius * c; // Distance in km
  return d;
}

function deg2rad(deg) {
  return deg * (Math.PI / 180);
}

function getClosestStation() {
  var smallestDistance = -1;
  var closestStation = null;
  lat1 = document.getElementById("lat").value;
  lon1 = document.getElementById("lng").value;
  for(var i = 0; i < stations.length; i++) {
    var distance = getDistanceFrom2GpsCoordinates(
      lat1,
      lon1,
      stations[i].position.lat,
      stations[i].position.lng
    );
    if (smallestDistance == -1 || distance < smallestDistance) {
      smallestDistance = distance;
      closestStation = stations[i];
    }
  }
  document.getElementById("closestStation").innerHTML = closestStation.name;
  console.log(closestStation);
}

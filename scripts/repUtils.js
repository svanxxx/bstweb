var state = 'block';

function showall(button) {
    divs = document.getElementsByTagName('div');
    for (k = 0; k < divs.length; k++) {
        if (divs[k].id.charAt(0) == 'F') {
            document.getElementById(divs[k].id).style.display = state;
        } 
    }
    state = (state == 'none') ? 'block' : 'none';
    document.getElementById('openall').innerHTML = (document.getElementById('openall').innerHTML == 'Open all') ? 'Close all' : 'Open all';
}

function switchview(id) {
     document.getElementById(id).style.display = (document.getElementById(id).style.display == 'none') ? 'block' : 'none'; 
}

function PutFiles() {
    var ValueText = '';
	var gitBranch = '';
    var strGuid = '';
    var strTestName = document.getElementById('ID_Test_Name').firstChild.nodeValue;

    var RUN_ID = document.URL.toString().split('&')[1];

    var listCheckedBox = document.getElementsByName('listCheckedBox');
    for (i = 0; i < listCheckedBox.length; i++) {
        if (listCheckedBox[i].checked == true) {
            ValueText = ValueText + listCheckedBox[i].value + '?';
        }
    }

    var gitBranches = document.getElementById('gitBranch');
        gitBranch = gitBranches.textContent;
    if (ValueText == '') {
        alert('Error: Files are not selected');
    }
    else {
        var data = {
            'lstFiles': ValueText
        };

        $.ajax({
            type: "POST",
            url: "PutFiles.aspx/GetListFiles",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                strGuid = response.d;
                document.location.href = "PutFiles.aspx?Guid=" + strGuid +"&Test_Name=" + strTestName + "&" + RUN_ID + "&gitBranch=" + gitBranch;
            },
            error: function (xhr, ajaxOptions, thrownError) { alert("Error:"+ xhr.responseText);}
        });
    }
}


function SetAllChecked() {
    var BtnSelet = document.getElementById("idSelectallfiles")
    var listCheckedBox = document.getElementsByName('listCheckedBox');

    bBox = false;
    if (BtnSelet.innerHTML== "Select all files") {
        bBox = true;
	BtnSelet.innerHTML = "Deselect all files";
    } 
     else BtnSelet.innerHTML = "Select all files";

    for (i = 0; i < listCheckedBox.length; i++) { listCheckedBox[i].checked = bBox; }
}

function SetAllNotChecked() {
    var listCheckedBox = document.getElementsByName('listCheckedBox');
    for (i = 0; i < listCheckedBox.length; i++) { listCheckedBox[i].checked = false; }
}

function Add_TT() {
    var RUN_ID = document.URL.toString().split('&')[1];
    document.location.href = 'CommentRun.aspx?' + RUN_ID; 
}

function TimeShow() {
    document.getElementById('idTimeButton').innerHTML = (document.getElementById('idTimeButton').innerHTML == 'Time OFF') ? 'Time ON' : 'Time OFF';

    stateTimeBlock = (document.getElementById('idTimeButton').innerHTML == 'Time ON') ? 'none' : 'inline-block';


    divs = document.getElementsByClassName('TimeZona');
    for (k = 0; k < divs.length; k++) {
            divs[k].style.display = stateTimeBlock;
    }
}

var StartBranchName = ""
function ChangeGitBranch() {
	var GitBranch = document.getElementById("gitBranch");
	if (StartBranchName == "") StartBranchName = GitBranch.innerHTML;
	
	var strBranchName = prompt("Enter a new name for the branch:");
	
	if (strBranchName.length == 0) strBranchName = StartBranchName;
	GitBranch.innerHTML = strBranchName;
}
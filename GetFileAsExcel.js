// JavaScript Document
function highlight_Table_Rows(table_Id, hover_Class, click_Class, multiple) {
var table = document.getElementById(table_Id);
if (typeof multiple == 'undefined') multiple = true;

if (hover_Class) {
 var hover_Class_Reg = new RegExp("\\b"+hover_Class+"\\b");
 table.onmouseover = table.onmouseout = function(e) {
  if (!e) e = window.event;
  var elem = e.target || e.srcElement;
  while (!elem.tagName || !elem.tagName.match(/td|th|table/i))
   elem = elem.parentNode;

  if (elem.parentNode.tagName == 'TR' &&
   elem.parentNode.parentNode.tagName == 'TBODY') {
   var row = elem.parentNode;
   if (!row.getAttribute('clicked_Row'))
   row.className = e.type=="mouseover"?row.className +
   " " + hover_Class:row.className.replace(hover_Class_Reg," ");
  }
 };
}

if (click_Class) table.onclick = function(e) {
 if (!e) e = window.event;
 var elem = e.target || e.srcElement;
 while (!elem.tagName || !elem.tagName.match(/td|th|table/i))
  elem = elem.parentNode;

 if (elem.parentNode.tagName == 'TR' &&
  elem.parentNode.parentNode.tagName == 'TBODY') {
  var click_Class_Reg = new RegExp("\\b"+click_Class+"\\b");
  var row = elem.parentNode;

  if (row.getAttribute('clicked_Row')) {
   row.removeAttribute('clicked_Row');
   row.className = row.className.replace(click_Class_Reg, "");
   row.className += " "+hover_Class;
  }
  else {
   if (hover_Class) row.className = row.className.replace(hover_Class_Reg, "");
   row.className += " "+click_Class;
   row.setAttribute('clicked_Row', true);

  if (!multiple) {
   var lastRowI = table.getAttribute("last_Clicked_Row");
   if (lastRowI!==null && lastRowI!=='' && row.sectionRowIndex!=lastRowI) {
    var lastRow = table.tBodies[0].rows[lastRowI];
    lastRow.className = lastRow.className.replace(click_Class_Reg, "");
    lastRow.removeAttribute('clicked_Row');
   }
  }
  table.setAttribute("last_Clicked_Row", row.sectionRowIndex);
  }
 }
};
}
$(document).ready(
  function() { 
    $(document).keypress(
      function(e) {
        if (e.target.tagName === "INPUT" || e.target.tagName === "TEXTAREA") {
          if (e.target.value.length > 0) {
            var dir = isRtl(e.target.value) ? "rtl" : "ltr";
            $(e.target).attr("dir", dir);
          }
        }
      }
    );
  }
);

function isRtl(text) {
  return text.length > 0 && isRtlCharCode(text.charCodeAt(0));
}

function isRtlCharCode(code) {
  return code >= 1568 && code <= 1919;
}

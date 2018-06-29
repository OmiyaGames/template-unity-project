mergeInto(LibraryManager.library, {
  RedirectTo: function (url) {
    window.top.location = "'" + url + "'";
  },
});

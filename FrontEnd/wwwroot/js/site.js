// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
function Delete(url) {
    Swal.fire({
      title: "Emin misiniz?",
      text: "Bunu geri döndüremezsiniz!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Evet, kaldır!",
      cancelButtonText: "Hayır, iptal et!"
    }).then((result) => {
      if (result.isConfirmed) {
        $.ajax({
          url: url,
          type: "DELETE",
          success: function () {
            window.location.reload();
          },
        });
      }
    });
  }


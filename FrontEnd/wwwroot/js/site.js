async function deleteBook(id) {
    const confirmed = confirm('Bu kitabı silmek istediğinizden emin misiniz?');
    if (!confirmed) return;

    try {
        const response = await fetch(`/Index?handler=Delete&id=${id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });

        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                alert('Kitap başarıyla silindi');
                location.reload();
            } else {
                alert('Failed to delete the book');
            }
        } else {
            alert('Silinirken hata oluştu');
        }
    } catch (error) {
        alert('İstek başarısız oldu: ' + error);
    }
}

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
          headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
          },
          success: function () {
            window.location.reload();
          },
        });
      }
    });
  }

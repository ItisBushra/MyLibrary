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
function confirmDelete(userId, showConfirm) {
    const confirmDeleteSpan = document.getElementById(`confirmDeleteSpan_${userId}`);
    const deleteSpan = document.getElementById(`deleteSpan_${userId}`);
    if (showConfirm) {
        confirmDeleteSpan.style.display = 'inline';
        deleteSpan.style.display = 'none';
    } else {
        confirmDeleteSpan.style.display = 'none';
        deleteSpan.style.display = 'inline';
    }
}
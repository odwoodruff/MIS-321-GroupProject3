// Security Check functionality
document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('securityCheckForm');
    const alertMessage = document.getElementById('alertMessage');

    form.addEventListener('submit', async function(e) {
        e.preventDefault();

        const formData = {
            fullName: document.getElementById('fullName').value.trim(),
            organizationName: document.getElementById('organizationName').value.trim(),
            department: document.getElementById('department').value.trim(),
            email: document.getElementById('email').value.trim(),
            role: document.getElementById('role').value.trim(),
            requestedAccessType: document.getElementById('requestedAccessType').value.trim()
        };

        // Validate all fields
        if (!formData.fullName || !formData.organizationName || !formData.department || 
            !formData.email || !formData.role || !formData.requestedAccessType) {
            showAlert('Please fill in all required fields.', 'danger');
            return;
        }

        // Validate email format
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(formData.email)) {
            showAlert('Please enter a valid email address.', 'danger');
            return;
        }

        try {
            // Ensure API_BASE_URL is defined
            const apiUrl = (typeof API_BASE_URL !== 'undefined' ? API_BASE_URL : '/api/api');
            const url = `${apiUrl}/access-requests`;
            
            console.log('Submitting access request to:', url);
            console.log('Request data:', formData);

            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            });

            let data;
            try {
                data = await response.json();
            } catch (jsonError) {
                // If response is not JSON, get text instead
                const text = await response.text();
                console.error('Non-JSON response:', text);
                showAlert(`Server error: ${response.status} ${response.statusText}. Please try again.`, 'danger');
                return;
            }

            if (response.ok) {
                showAlert(data.message || 'Access request submitted successfully! Awaiting admin approval.', 'success');
                form.reset();
                
                // Redirect to a waiting page or show message
                setTimeout(() => {
                    alert('Your access request has been submitted. You will be notified once an administrator reviews your request.');
                    // In a real application, you might redirect to a waiting page
                    // window.location.href = 'waiting.html';
                }, 2000);
            } else {
                showAlert(data.message || `Error submitting access request (${response.status}). Please try again.`, 'danger');
            }
        } catch (error) {
            console.error('Error submitting access request:', error);
            if (error.message.includes('Failed to fetch') || error.message.includes('NetworkError')) {
                showAlert('Cannot connect to server. Please ensure the server is running and try again.', 'danger');
            } else {
                showAlert(`Error: ${error.message}. Please check your connection and try again.`, 'danger');
            }
        }
    });
});

function showAlert(message, type) {
    const alertMessage = document.getElementById('alertMessage');
    alertMessage.className = `alert alert-${type} alert-message`;
    alertMessage.textContent = message;
    alertMessage.style.display = 'block';
    
    // Auto-hide after 5 seconds
    setTimeout(() => {
        alertMessage.style.display = 'none';
    }, 5000);
}


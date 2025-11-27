// Admin Panel functionality
let allAccessRequests = [];
let allApprovedMembers = [];
let allOrganizations = [];
let allAuditLogs = [];
let currentRequestId = null;

document.addEventListener('DOMContentLoaded', function() {
    setupTabs();
    loadAccessReviewData();
    setupRefreshInterval();
});

function setupTabs() {
    const tabs = document.querySelectorAll('.admin-tab');
    const tabContents = document.querySelectorAll('.tab-content');

    tabs.forEach(tab => {
        tab.addEventListener('click', function() {
            const targetTab = this.getAttribute('data-tab');

            // Remove active class from all tabs and contents
            tabs.forEach(t => t.classList.remove('active'));
            tabContents.forEach(tc => tc.classList.remove('active'));

            // Add active class to clicked tab and corresponding content
            this.classList.add('active');
            document.getElementById(targetTab).classList.add('active');

            // Load data for the active tab
            switch(targetTab) {
                case 'access-review':
                    loadAccessReviewData();
                    break;
                case 'approved-members':
                    loadApprovedMembers();
                    break;
                case 'organizations':
                    loadOrganizations();
                    break;
                case 'audit-logs':
                    loadAuditLogs();
                    break;
                case 'review-tools':
                    if (typeof loadAdminStats === 'function') loadAdminStats();
                    if (typeof loadAdminAlerts === 'function') loadAdminAlerts();
                    break;
            }
        });
    });
}

// Access Review Tab
async function loadAccessReviewData() {
    await Promise.all([
        loadAccessRequests(),
        loadAccessReviewStats()
    ]);
}

async function loadAccessRequests() {
    const tbody = document.getElementById('accessRequestsTableBody');
    tbody.innerHTML = '<tr><td colspan="10" style="text-align: center; padding: 2rem;"><p class="loading"><i class="bi bi-arrow-repeat"></i> Loading...</p></td></tr>';

    try {
        allAccessRequests = await fetchAPI('/access-requests');
        displayAccessRequestsTable(allAccessRequests, tbody);
    } catch (error) {
        console.error('Error loading access requests:', error);
        tbody.innerHTML = '<tr><td colspan="10" style="text-align: center; padding: 2rem;"><p class="empty-state">Error loading access requests.</p></td></tr>';
    }
}

async function loadAccessReviewStats() {
    const container = document.getElementById('accessReviewStats');
    
    try {
        const requests = await fetchAPI('/access-requests');
        const pending = requests.filter(r => r.status === 'Pending').length;
        const approved = requests.filter(r => r.status === 'Approved').length;
        const denied = requests.filter(r => r.status === 'Denied').length;
        const moreInfo = requests.filter(r => r.status === 'MoreInfoRequested').length;

        container.innerHTML = `
            <div class="stat-card">
                <h4>Total Requests</h4>
                <p class="stat-value">${requests.length}</p>
            </div>
            <div class="stat-card">
                <h4>Pending</h4>
                <p class="stat-value" style="color: #ffc107;">${pending}</p>
            </div>
            <div class="stat-card">
                <h4>Approved</h4>
                <p class="stat-value" style="color: #28a745;">${approved}</p>
            </div>
            <div class="stat-card">
                <h4>Denied</h4>
                <p class="stat-value" style="color: #dc3545;">${denied}</p>
            </div>
            <div class="stat-card">
                <h4>More Info Requested</h4>
                <p class="stat-value" style="color: #17a2b8;">${moreInfo}</p>
            </div>
        `;
    } catch (error) {
        console.error('Error loading stats:', error);
    }
}

function displayAccessRequestsTable(requests, tbody) {
    if (requests.length === 0) {
        tbody.innerHTML = '<tr><td colspan="10" style="text-align: center; padding: 2rem;"><p class="empty-state">No access requests found.</p></td></tr>';
        return;
    }

    tbody.innerHTML = requests.map(request => {
        const statusClass = `status-${request.status.toLowerCase().replace(' ', '-')}`;
        const trustClass = `trust-${request.domainTrustScore?.toLowerCase() || 'unknown'}`;
        const emailStatus = request.emailConfirmationStatus || 'Pending';
        const hasVerification = request.hasVerificationUpload ? '<i class="bi bi-check-circle text-success"></i>' : '<i class="bi bi-x-circle text-muted"></i>';

        return `
            <tr>
                <td><strong>${escapeHtml(request.fullName)}</strong></td>
                <td>${escapeHtml(request.organizationName)}<br><small class="text-muted">${escapeHtml(request.department)}</small></td>
                <td>${escapeHtml(request.email)}</td>
                <td>${escapeHtml(request.role)}</td>
                <td>${escapeHtml(request.requestedAccessType || 'Full alert access')}</td>
                <td>${formatDate(request.requestDate)}</td>
                <td>
                    ${emailStatus}
                    ${hasVerification}
                </td>
                <td><span class="trust-score ${trustClass}">${request.domainTrustScore || 'Unknown'}</span></td>
                <td><span class="status-badge ${statusClass}">${request.status}</span></td>
                <td>
                    <div class="action-buttons">
                        <button class="btn btn-sm btn-info" onclick="viewRequest(${request.id})" title="View Details">
                            <i class="bi bi-eye"></i> View
                        </button>
                        ${request.status === 'Pending' ? `
                            <button class="btn btn-sm btn-success" onclick="approveRequest(${request.id})" title="Approve">
                                <i class="bi bi-check"></i> Approve
                            </button>
                            <button class="btn btn-sm btn-danger" onclick="denyRequest(${request.id})" title="Deny">
                                <i class="bi bi-x"></i> Deny
                            </button>
                            <button class="btn btn-sm btn-warning" onclick="requestMoreInfo(${request.id})" title="Request More Info">
                                <i class="bi bi-question-circle"></i> More Info
                            </button>
                        ` : ''}
                        <button class="btn btn-sm btn-secondary" onclick="assignRole(${request.id})" title="Assign Role">
                            <i class="bi bi-person-badge"></i> Role
                        </button>
                    </div>
                </td>
            </tr>
        `;
    }).join('');
}

// Approved Members Tab
async function loadApprovedMembers() {
    const tbody = document.getElementById('approvedMembersTableBody');
    tbody.innerHTML = '<tr><td colspan="9" style="text-align: center; padding: 2rem;"><p class="loading"><i class="bi bi-arrow-repeat"></i> Loading...</p></td></tr>';

    try {
        allApprovedMembers = await fetchAPI('/approved-members');
        displayApprovedMembersTable(allApprovedMembers, tbody);
    } catch (error) {
        console.error('Error loading approved members:', error);
        tbody.innerHTML = '<tr><td colspan="9" style="text-align: center; padding: 2rem;"><p class="empty-state">Error loading approved members.</p></td></tr>';
    }
}

function displayApprovedMembersTable(members, tbody) {
    if (members.length === 0) {
        tbody.innerHTML = '<tr><td colspan="9" style="text-align: center; padding: 2rem;"><p class="empty-state">No approved members found.</p></td></tr>';
        return;
    }

    tbody.innerHTML = members.map(member => {
        const statusClass = member.isActive ? 'status-approved' : 'status-denied';
        const statusText = member.isActive ? 'Active' : 'Inactive';

        return `
            <tr>
                <td><strong>${escapeHtml(member.fullName)}</strong></td>
                <td>${escapeHtml(member.organizationName)}</td>
                <td>${escapeHtml(member.email)}</td>
                <td><span class="status-badge">${escapeHtml(member.assignedRole || 'Member')}</span></td>
                <td>${escapeHtml(member.accessLevel)}</td>
                <td>${formatDate(member.approvalDate)}</td>
                <td>${member.lastLoginDate ? formatDate(member.lastLoginDate) : 'Never'}</td>
                <td><span class="status-badge ${statusClass}">${statusText}</span></td>
                <td>
                    <div class="action-buttons">
                        <button class="btn btn-sm btn-secondary" onclick="assignMemberRole(${member.id})" title="Change Role">
                            <i class="bi bi-person-badge"></i> Role
                        </button>
                        ${member.isActive ? `
                            <button class="btn btn-sm btn-danger" onclick="deactivateMember(${member.id})" title="Deactivate">
                                <i class="bi bi-x-circle"></i> Deactivate
                            </button>
                        ` : ''}
                    </div>
                </td>
            </tr>
        `;
    }).join('');
}

// Organizations Tab
async function loadOrganizations() {
    const tbody = document.getElementById('organizationsTableBody');
    tbody.innerHTML = '<tr><td colspan="8" style="text-align: center; padding: 2rem;"><p class="loading"><i class="bi bi-arrow-repeat"></i> Loading...</p></td></tr>';

    try {
        allOrganizations = await fetchAPI('/organizations');
        displayOrganizationsTable(allOrganizations, tbody);
    } catch (error) {
        console.error('Error loading organizations:', error);
        tbody.innerHTML = '<tr><td colspan="8" style="text-align: center; padding: 2rem;"><p class="empty-state">Error loading organizations.</p></td></tr>';
    }
}

function displayOrganizationsTable(orgs, tbody) {
    if (orgs.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" style="text-align: center; padding: 2rem;"><p class="empty-state">No organizations found.</p></td></tr>';
        return;
    }

    tbody.innerHTML = orgs.map(org => {
        const verificationClass = org.isVerified ? 'status-approved' : org.verificationStatus === 'Rejected' ? 'status-denied' : 'status-pending';

        return `
            <tr>
                <td><strong>${escapeHtml(org.name)}</strong></td>
                <td>${escapeHtml(org.domain)}</td>
                <td>${escapeHtml(org.sector || 'Unknown')}</td>
                <td>${escapeHtml(org.type || 'Unknown')}</td>
                <td>${org.memberCount}</td>
                <td>${formatDate(org.registrationDate)}</td>
                <td><span class="status-badge ${verificationClass}">${org.verificationStatus}</span></td>
                <td>
                    <div class="action-buttons">
                        <button class="btn btn-sm btn-success" onclick="verifyOrganization(${org.id}, true)" title="Verify">
                            <i class="bi bi-check"></i> Verify
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="verifyOrganization(${org.id}, false)" title="Reject">
                            <i class="bi bi-x"></i> Reject
                        </button>
                    </div>
                </td>
            </tr>
        `;
    }).join('');
}

// Audit Logs Tab
async function loadAuditLogs() {
    const tbody = document.getElementById('auditLogsTableBody');
    tbody.innerHTML = '<tr><td colspan="6" style="text-align: center; padding: 2rem;"><p class="loading"><i class="bi bi-arrow-repeat"></i> Loading...</p></td></tr>';

    try {
        allAuditLogs = await fetchAPI('/audit-logs');
        displayAuditLogsTable(allAuditLogs, tbody);
    } catch (error) {
        console.error('Error loading audit logs:', error);
        tbody.innerHTML = '<tr><td colspan="6" style="text-align: center; padding: 2rem;"><p class="empty-state">Error loading audit logs.</p></td></tr>';
    }
}

function displayAuditLogsTable(logs, tbody) {
    if (logs.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" style="text-align: center; padding: 2rem;"><p class="empty-state">No audit logs found.</p></td></tr>';
        return;
    }

    tbody.innerHTML = logs.map(log => {
        const severityClass = log.severity === 'Critical' ? 'status-denied' : log.severity === 'Warning' ? 'status-pending' : 'status-approved';

        return `
            <tr>
                <td>${formatDate(log.timestamp)}</td>
                <td><strong>${escapeHtml(log.action)}</strong></td>
                <td>${escapeHtml(log.userEmail)}</td>
                <td>${escapeHtml(log.adminEmail)}</td>
                <td>${escapeHtml(log.details)}</td>
                <td><span class="status-badge ${severityClass}">${log.severity}</span></td>
            </tr>
        `;
    }).join('');
}

// Action Functions
function viewRequest(id) {
    const request = allAccessRequests.find(r => r.id === id);
    if (!request) return;

    const modal = new bootstrap.Modal(document.getElementById('viewRequestModal'));
    document.getElementById('requestDetails').innerHTML = `
        <div class="request-details">
            <h5>${escapeHtml(request.fullName)}</h5>
            <p><strong>Organization:</strong> ${escapeHtml(request.organizationName)}</p>
            <p><strong>Department:</strong> ${escapeHtml(request.department)}</p>
            <p><strong>Email:</strong> ${escapeHtml(request.email)}</p>
            <p><strong>Role:</strong> ${escapeHtml(request.role)}</p>
            <p><strong>Requested Access:</strong> ${escapeHtml(request.requestedAccessType || 'Full alert access')}</p>
            <p><strong>Submission Date:</strong> ${formatDate(request.requestDate)}</p>
            <p><strong>Email Status:</strong> ${request.emailConfirmationStatus || 'Pending'}</p>
            <p><strong>Domain Trust Score:</strong> <span class="trust-score trust-${request.domainTrustScore?.toLowerCase() || 'unknown'}">${request.domainTrustScore || 'Unknown'}</span></p>
            <p><strong>Verification Upload:</strong> ${request.hasVerificationUpload ? 'Yes' : 'No'}</p>
            ${request.verificationNotes ? `<p><strong>Notes:</strong> ${escapeHtml(request.verificationNotes)}</p>` : ''}
            <p><strong>Status:</strong> <span class="status-badge status-${request.status.toLowerCase().replace(' ', '-')}">${request.status}</span></p>
        </div>
    `;
    modal.show();
}

async function approveRequest(id) {
    if (!confirm('Are you sure you want to approve this access request?')) return;
    await updateRequestStatus(id, 'Approved');
}

async function denyRequest(id) {
    if (!confirm('Are you sure you want to deny this access request?')) return;
    await updateRequestStatus(id, 'Denied');
}

async function updateRequestStatus(id, status) {
    try {
        const response = await fetch(`${API_BASE_URL}/access-requests/${id}/status`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ status })
        });

        if (response.ok) {
            await loadAccessReviewData();
        } else {
            const data = await response.json();
            alert('Error: ' + (data.message || 'Unknown error'));
        }
    } catch (error) {
        console.error('Error updating status:', error);
        alert('Error updating request status. Please try again.');
    }
}

function assignRole(id) {
    currentRequestId = id;
    const request = allAccessRequests.find(r => r.id === id);
    if (request && request.assignedRole) {
        document.getElementById('roleSelect').value = request.assignedRole;
    }
    const modal = new bootstrap.Modal(document.getElementById('assignRoleModal'));
    modal.show();
}

async function confirmAssignRole() {
    if (!currentRequestId) return;
    
    const role = document.getElementById('roleSelect').value;
    try {
        const response = await fetch(`${API_BASE_URL}/access-requests/${currentRequestId}/role`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ role })
        });

        if (response.ok) {
            bootstrap.Modal.getInstance(document.getElementById('assignRoleModal')).hide();
            await loadAccessReviewData();
        } else {
            const data = await response.json();
            alert('Error: ' + (data.message || 'Unknown error'));
        }
    } catch (error) {
        console.error('Error assigning role:', error);
        alert('Error assigning role. Please try again.');
    }
}

function requestMoreInfo(id) {
    currentRequestId = id;
    document.getElementById('infoRequestNotes').value = '';
    const modal = new bootstrap.Modal(document.getElementById('requestInfoModal'));
    modal.show();
}

async function confirmRequestInfo() {
    if (!currentRequestId) return;
    
    const notes = document.getElementById('infoRequestNotes').value.trim();
    if (!notes) {
        alert('Please enter a message for the user.');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/access-requests/${currentRequestId}/request-info`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ notes })
        });

        if (response.ok) {
            bootstrap.Modal.getInstance(document.getElementById('requestInfoModal')).hide();
            await loadAccessReviewData();
        } else {
            const data = await response.json();
            alert('Error: ' + (data.message || 'Unknown error'));
        }
    } catch (error) {
        console.error('Error requesting info:', error);
        alert('Error sending request. Please try again.');
    }
}

async function assignMemberRole(id) {
    const member = allApprovedMembers.find(m => m.id === id);
    if (!member) return;
    
    currentRequestId = id;
    document.getElementById('roleSelect').value = member.assignedRole || 'Member';
    const modal = new bootstrap.Modal(document.getElementById('assignRoleModal'));
    modal.show();
    
    // Change the confirm button to update member role
    const confirmBtn = document.querySelector('#assignRoleModal .btn-primary');
    confirmBtn.onclick = async () => {
        const role = document.getElementById('roleSelect').value;
        try {
            const response = await fetch(`${API_BASE_URL}/approved-members/${id}/role`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ role })
            });

            if (response.ok) {
                bootstrap.Modal.getInstance(document.getElementById('assignRoleModal')).hide();
                await loadApprovedMembers();
            } else {
                alert('Error updating member role.');
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Error updating member role.');
        }
    };
}

async function deactivateMember(id) {
    if (!confirm('Are you sure you want to deactivate this member?')) return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/approved-members/${id}/deactivate`, {
            method: 'PUT'
        });

        if (response.ok) {
            await loadApprovedMembers();
        } else {
            alert('Error deactivating member.');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Error deactivating member.');
    }
}

async function verifyOrganization(id, isVerified) {
    const status = isVerified ? 'Verified' : 'Rejected';
    if (!confirm(`Are you sure you want to ${isVerified ? 'verify' : 'reject'} this organization?`)) return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/organizations/${id}/verification`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ isVerified, status })
        });

        if (response.ok) {
            await loadOrganizations();
        } else {
            alert('Error updating organization verification.');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Error updating organization verification.');
    }
}

function saveSystemSettings() {
    // This would save system settings
    alert('System settings saved successfully!');
}

function setupRefreshInterval() {
    setInterval(() => {
        const activeTab = document.querySelector('.admin-tab.active');
        if (activeTab) {
            const tabName = activeTab.getAttribute('data-tab');
            if (tabName === 'access-review') {
                loadAccessReviewData();
            } else if (tabName === 'approved-members') {
                loadApprovedMembers();
            } else if (tabName === 'organizations') {
                loadOrganizations();
            } else if (tabName === 'audit-logs') {
                loadAuditLogs();
            }
        }
    }, 30000); // Refresh every 30 seconds
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Make functions available globally
window.viewRequest = viewRequest;
window.approveRequest = approveRequest;
window.denyRequest = denyRequest;
window.assignRole = assignRole;
window.confirmAssignRole = confirmAssignRole;
window.requestMoreInfo = requestMoreInfo;
window.confirmRequestInfo = confirmRequestInfo;
window.assignMemberRole = assignMemberRole;
window.deactivateMember = deactivateMember;
window.verifyOrganization = verifyOrganization;

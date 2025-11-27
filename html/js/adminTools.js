// Admin Review Tools functionality
document.addEventListener('DOMContentLoaded', async function() {
    await loadAdminStats();
    await loadAdminAlerts();
    setupAdminActions();
});

async function loadAdminStats() {
    const container = document.getElementById('adminStats');
    
    try {
        const metrics = await fetchAPI('/metrics');
        
        container.innerHTML = `
            <div class="stat-card">
                <h4>Total Alerts</h4>
                <p class="stat-value">${metrics.totalAlerts}</p>
            </div>
            <div class="stat-card">
                <h4>Agriculture</h4>
                <p class="stat-value">${metrics.agricultureAlerts}</p>
            </div>
            <div class="stat-card">
                <h4>Hospitals</h4>
                <p class="stat-value">${metrics.hospitalAlerts}</p>
            </div>
            <div class="stat-card">
                <h4>Pharma</h4>
                <p class="stat-value">${metrics.pharmaAlerts}</p>
            </div>
            <div class="stat-card">
                <h4>Other Sectors</h4>
                <p class="stat-value">${metrics.otherAlerts}</p>
            </div>
            <div class="stat-card">
                <h4>Last Updated</h4>
                <p class="stat-value">${formatDate(metrics.lastUpdated)}</p>
            </div>
        `;
    } catch (error) {
        console.error('Error loading admin stats:', error);
        container.innerHTML = '<p class="empty-state">Error loading statistics.</p>';
    }
}

async function loadAdminAlerts() {
    const container = document.getElementById('adminAlerts');
    container.innerHTML = '<p class="loading">Loading alerts...</p>';
    
    try {
        const alerts = await fetchAPI('/alerts');
        displayAdminAlerts(alerts, container);
    } catch (error) {
        console.error('Error loading admin alerts:', error);
        container.innerHTML = 
            '<p class="empty-state">Error loading alerts. Please try again later.</p>';
    }
}

function displayAdminAlerts(alerts, container) {
    if (alerts.length === 0) {
        container.innerHTML = '<p class="empty-state">No alerts available.</p>';
        return;
    }
    
    container.innerHTML = `
        <div class="admin-alerts-header">
            <p>Total: ${alerts.length} alerts</p>
        </div>
        <div class="alert-feed">
            ${alerts.map(alert => `
                <div class="alert-card ${getSeverityClass(alert.severity)}">
                    <div class="alert-header">
                        <div>
                            <div class="alert-title">[ID: ${alert.id}] ${escapeHtml(alert.title)}</div>
                            <div class="alert-meta">
                                <span><strong>Sector:</strong> ${escapeHtml(alert.sector)}</span>
                                <span><strong>Severity:</strong> ${escapeHtml(alert.severity)}</span>
                                <span><strong>Source:</strong> ${escapeHtml(alert.source)}</span>
                                <span><strong>Created:</strong> ${formatDate(alert.dateCreated)}</span>
                            </div>
                        </div>
                    </div>
                    <div class="alert-description">${escapeHtml(alert.description)}</div>
                    <div class="alert-tags">
                        ${alert.tags.map(tag => 
                            `<span class="tag ${tag.toLowerCase()}">${escapeHtml(tag)}</span>`
                        ).join('')}
                    </div>
                </div>
            `).join('')}
        </div>
    `;
}

function setupAdminActions() {
    const refreshBtn = document.getElementById('refreshAlerts');
    const exportBtn = document.getElementById('exportAlerts');
    
    refreshBtn.addEventListener('click', async function() {
        this.textContent = 'Refreshing...';
        this.disabled = true;
        
        try {
            await loadAdminStats();
            await loadAdminAlerts();
        } catch (error) {
            console.error('Error refreshing:', error);
            alert('Error refreshing alerts. Please try again.');
        } finally {
            this.textContent = 'Refresh All Alerts';
            this.disabled = false;
        }
    });
    
    exportBtn.addEventListener('click', function() {
        // In a real implementation, this would export alerts to CSV/JSON
        alert('Export functionality will be implemented in a future update.');
    });
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}


// Dashboard functionality
document.addEventListener('DOMContentLoaded', async function() {
    await loadDashboardMetrics();
    await loadAlerts('All');
    setupFilterButtons();
});

async function loadDashboardMetrics() {
    try {
        const metrics = await fetchAPI('/metrics');
        
        document.getElementById('totalAlerts').textContent = metrics.totalAlerts;
        document.getElementById('filteredAlerts').textContent = metrics.filteredAlerts;
        document.getElementById('agricultureAlerts').textContent = metrics.agricultureAlerts;
        document.getElementById('hospitalAlerts').textContent = metrics.hospitalAlerts;
        document.getElementById('pharmaAlerts').textContent = metrics.pharmaAlerts;
        document.getElementById('otherAlerts').textContent = metrics.otherAlerts;
        document.getElementById('lastUpdated').textContent = formatDate(metrics.lastUpdated);
    } catch (error) {
        console.error('Error loading metrics:', error);
        document.getElementById('alertFeed').innerHTML = 
            '<p class="empty-state">Error loading dashboard metrics. Please try again later.</p>';
    }
}

async function loadAlerts(sector) {
    const alertFeed = document.getElementById('alertFeed');
    alertFeed.innerHTML = '<p class="loading">Loading alerts...</p>';
    
    try {
        const alerts = await fetchAPI(`/alerts?sector=${encodeURIComponent(sector)}`);
        displayAlerts(alerts, alertFeed);
    } catch (error) {
        console.error('Error loading alerts:', error);
        alertFeed.innerHTML = 
            '<p class="empty-state">Error loading alerts. Please try again later.</p>';
    }
}

function displayAlerts(alerts, container) {
    if (alerts.length === 0) {
        container.innerHTML = '<p class="empty-state">No alerts found for the selected sector.</p>';
        return;
    }
    
    container.innerHTML = alerts.map(alert => `
        <div class="alert-card ${getSeverityClass(alert.severity)}">
            <div class="alert-header">
                <div>
                    <div class="alert-title">${escapeHtml(alert.title)}</div>
                    <div class="alert-meta">
                        <span><strong>Sector:</strong> ${escapeHtml(alert.sector)}</span>
                        <span><strong>Severity:</strong> ${escapeHtml(alert.severity)}</span>
                        <span><strong>Source:</strong> ${escapeHtml(alert.source)}</span>
                        <span><strong>Date:</strong> ${formatDate(alert.dateCreated)}</span>
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
    `).join('');
}

function setupFilterButtons() {
    const filterButtons = document.querySelectorAll('.filter-btn');
    
    filterButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Remove active class from all buttons
            filterButtons.forEach(btn => btn.classList.remove('active'));
            // Add active class to clicked button
            this.classList.add('active');
            
            const sector = this.getAttribute('data-sector');
            loadAlerts(sector);
        });
    });
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}


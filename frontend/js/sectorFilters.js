// Sector Filters functionality
document.addEventListener('DOMContentLoaded', function() {
    setupSectorFilter();
});

function setupSectorFilter() {
    const applyButton = document.getElementById('applyFilter');
    const sectorSelect = document.getElementById('sectorSelect');
    
    applyButton.addEventListener('click', async function() {
        const sector = sectorSelect.value;
        await loadFilteredAlerts(sector);
    });
    
    // Also load on page load
    loadFilteredAlerts('All');
}

async function loadFilteredAlerts(sector) {
    const container = document.getElementById('filteredAlerts');
    container.innerHTML = '<p class="loading">Loading filtered alerts...</p>';
    
    try {
        const alerts = await fetchAPI(`/alerts?sector=${encodeURIComponent(sector)}`);
        displayFilteredAlerts(alerts, container, sector);
    } catch (error) {
        console.error('Error loading filtered alerts:', error);
        container.innerHTML = 
            '<p class="empty-state">Error loading alerts. Please try again later.</p>';
    }
}

function displayFilteredAlerts(alerts, container, sector) {
    if (alerts.length === 0) {
        container.innerHTML = `<p class="empty-state">No alerts found for ${sector} sector.</p>`;
        return;
    }
    
    container.innerHTML = `
        <div class="filter-results-header">
            <h3>Showing ${alerts.length} alert(s) for ${sector}</h3>
        </div>
        <div class="alert-feed">
            ${alerts.map(alert => `
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
            `).join('')}
        </div>
    `;
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}


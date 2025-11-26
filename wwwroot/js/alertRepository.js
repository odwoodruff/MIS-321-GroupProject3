// Alert Repository functionality
let allAlerts = [];

document.addEventListener('DOMContentLoaded', async function() {
    await loadAllAlerts();
    setupSearch();
});

async function loadAllAlerts() {
    const container = document.getElementById('repositoryAlerts');
    container.innerHTML = '<p class="loading">Loading all alerts...</p>';
    
    try {
        allAlerts = await fetchAPI('/alerts');
        displayRepositoryAlerts(allAlerts, container);
    } catch (error) {
        console.error('Error loading alerts:', error);
        container.innerHTML = 
            '<p class="empty-state">Error loading alerts. Please try again later.</p>';
    }
}

function displayRepositoryAlerts(alerts, container) {
    if (alerts.length === 0) {
        container.innerHTML = '<p class="empty-state">No alerts available in the repository.</p>';
        return;
    }
    
    container.innerHTML = `
        <div class="repository-header">
            <h3>Total Alerts: ${alerts.length}</h3>
        </div>
        <div class="alert-feed">
            ${alerts.map(alert => `
                <div class="alert-card ${getSeverityClass(alert.severity)}">
                    <div class="alert-header">
                        <div>
                            <div class="alert-title">${escapeHtml(alert.title)}</div>
                            <div class="alert-meta">
                                <span><strong>ID:</strong> ${alert.id}</span>
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

function setupSearch() {
    const searchInput = document.getElementById('searchInput');
    const searchBtn = document.getElementById('searchBtn');
    
    const performSearch = () => {
        const searchTerm = searchInput.value.toLowerCase().trim();
        
        if (searchTerm === '') {
            displayRepositoryAlerts(allAlerts, document.getElementById('repositoryAlerts'));
            return;
        }
        
        const filtered = allAlerts.filter(alert => 
            alert.title.toLowerCase().includes(searchTerm) ||
            alert.description.toLowerCase().includes(searchTerm) ||
            alert.sector.toLowerCase().includes(searchTerm) ||
            alert.tags.some(tag => tag.toLowerCase().includes(searchTerm))
        );
        
        displayRepositoryAlerts(filtered, document.getElementById('repositoryAlerts'));
    };
    
    searchBtn.addEventListener('click', performSearch);
    searchInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            performSearch();
        }
    });
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}


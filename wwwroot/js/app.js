// Global State
let connection = null;
let currentPreviewIp = null;
let currentPreviewStream = null;
let globalConfig = null;
let localConfigCameras = [];
let statusInterval = null;

// Initialization
document.addEventListener("DOMContentLoaded", () => {
    // 1. Initialize SignalR
    initSignalR();

    // 2. Initial Data Pull
    fetchStatus();

    // 3. Start Polling Status
    statusInterval = setInterval(fetchStatus, 1000);
});

// Routing & Tab Switching
function switchTab(tabId) {
    // Toggle active menu button
    document.querySelectorAll(".menu-item").forEach(btn => {
        btn.classList.remove("active");
    });
    const activeBtn = Array.from(document.querySelectorAll(".menu-item")).find(btn => 
        btn.getAttribute("onclick").includes(`'${tabId}'`)
    );
    if (activeBtn) activeBtn.classList.add("active");

    // Toggle active panel
    document.querySelectorAll(".tab-pane").forEach(pane => {
        pane.classList.remove("active");
    });
    document.getElementById(`tab-${tabId}`).classList.add("active");

    // Fill settings inputs when switching to settings tab
    if (tabId === 'settings') {
        loadNetworkInterfaces().then(() => {
            if (globalConfig) {
                populateSettingsForm();
            }
        });
    }
}

// SignalR WebSocket Log Subscriptions
function initSignalR() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/streamHub")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveLog", (time, level, message) => {
        appendLogLine(time, level, message);
    });

    connection.start()
        .then(() => {
            appendLogLine(getCurrentTimeStr(), "INFO", "SignalR WebSocket connected successfully.");
        })
        .catch(err => {
            appendLogLine(getCurrentTimeStr(), "ERROR", "SignalR WebSocket connection failed: " + err.toString());
        });
}

function appendLogLine(time, level, message) {
    const consoleBox = document.getElementById("console-output");
    if (!consoleBox) return;

    // Limit log rows
    if (consoleBox.children.length > 500) {
        consoleBox.removeChild(consoleBox.firstChild);
    }

    const logLine = document.createElement("div");
    logLine.className = "log-line";

    const logTime = document.createElement("span");
    logTime.className = "log-time";
    logTime.textContent = `[${time}]`;

    const logLevel = document.createElement("span");
    logLevel.className = `log-level log-level-${level.toLowerCase()}`;
    logLevel.textContent = `[${level}]`;

    const logMsg = document.createElement("span");
    logMsg.className = "log-msg";
    logMsg.textContent = message;

    logLine.appendChild(logTime);
    logLine.appendChild(logLevel);
    logLine.appendChild(logMsg);

    consoleBox.appendChild(logLine);

    // Auto-scroll
    consoleBox.scrollTop = consoleBox.scrollHeight;
}

function clearWebConsole() {
    const consoleBox = document.getElementById("console-output");
    if (consoleBox) consoleBox.innerHTML = "";
}

function getCurrentTimeStr() {
    const now = new Date();
    return now.toTimeString().split(' ')[0];
}

// Fetch Status & Render UI
function fetchStatus() {
    fetch("/api/status")
        .then(response => {
            if (!response.ok) throw new Error("HTTP error " + response.status);
            return response.json();
        })
        .then(data => {
            // Save state globally
            globalConfig = {
                ffmpegPath: data.ffmpegPath || globalConfig?.ffmpegPath || "Binaries\\ffmpeg.exe",
                mediamtxPath: data.mediamtxPath || globalConfig?.mediamtxPath || "Binaries\\mediamtx.exe",
                mediamtxPort: data.mediamtxPort,
                webApiPort: 5000,
                selectedInterfaceIp: data.selectedInterfaceIp || "Auto",
                cameras: data.cameras.map(c => ({
                    ipAddress: c.ipAddress,
                    streamName: c.streamName,
                    codec: c.codec,
                    encoderMode: c.encoder,
                    targetFps: c.targetFps,
                    packetSize: c.packetSize,
                    interPacketDelay: c.interPacketDelay
                }))
            };

            // If localConfigCameras is empty, sync it
            if (localConfigCameras.length === 0 && globalConfig.cameras.length > 0) {
                localConfigCameras = JSON.parse(JSON.stringify(globalConfig.cameras));
                renderSettingsCameraGrid();
            }

            // Update MediaMTX server status
            updateServerStatusUI(data.mediamtxRunning, data.mediamtxPort);

            // Render camera dashboard grid
            renderDashboardCameraGrid(data.cameras);
        })
        .catch(err => {
            console.error("Failed to fetch API status:", err);
        });
}

function updateServerStatusUI(running, port) {
    const badge = document.getElementById("mtx-status-badge");
    const btnStart = document.getElementById("btn-start");
    const btnStop = document.getElementById("btn-stop");

    if (running) {
        badge.textContent = `Running (Port ${port})`;
        badge.className = "badge badge-success";
        btnStart.disabled = true;
        btnStop.disabled = false;
    } else {
        badge.textContent = "Stopped";
        badge.className = "badge badge-warning";
        btnStart.disabled = false;
        btnStop.disabled = true;
    }
}

function renderDashboardCameraGrid(cameras) {
    const tbody = document.getElementById("camera-tbody");
    if (!tbody) return;

    if (cameras.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-muted">No cameras loaded. Click Settings to configure.</td></tr>`;
        return;
    }

    let html = "";
    cameras.forEach(cam => {
        const host = (globalConfig?.selectedInterfaceIp && globalConfig.selectedInterfaceIp !== "Auto")
            ? globalConfig.selectedInterfaceIp
            : (window.location.hostname || "localhost");
        const rtspUrl = `rtsp://${host}:${globalConfig?.mediamtxPort || 8554}/${cam.streamName}`;
        const encoderText = `${cam.codec} (${cam.encoder})`;
        const fpsText = `${cam.targetFps} / ${cam.fps} FPS`;

        let statusClass = "badge-warning";
        if (cam.status === "Streaming" || cam.status === "Grabbing") statusClass = "badge-success";
        if (cam.status === "Disconnected" || cam.status === "Error") statusClass = "badge-danger";

        const isActive = (currentPreviewIp === cam.ipAddress) ? "class='active-preview-row'" : "";

        // Action restart button
        const restartBtnDisabled = cam.status === "Stopped" ? "disabled" : "";

        html += `
            <tr ${isActive} onclick="selectCameraPreview('${cam.ipAddress}', '${cam.streamName}', '${cam.status}')">
                <td style="font-weight: 600;">${cam.ipAddress}</td>
                <td>
                    <div style="display: flex; align-items: center; gap: 8px;">
                        <code style="color: #10b981; font-family: monospace;">${rtspUrl}</code>
                        <button class="btn btn-secondary btn-xs" onclick="copyToClipboard(event, '${rtspUrl}')">Copy</button>
                    </div>
                </td>
                <td>${encoderText}</td>
                <td>${fpsText}</td>
                <td><span class="badge ${statusClass}">${cam.status}</span></td>
                <td>
                    <button class="btn btn-secondary btn-xs" onclick="restartCamera(event, '${cam.ipAddress}')" ${restartBtnDisabled}>
                        Restart Pipe
                    </button>
                </td>
            </tr>
        `;
    });

    tbody.innerHTML = html;
}

// MediaMTX WebRTC Side Preview Loading
function selectCameraPreview(ip, streamName, status) {
    currentPreviewIp = ip;
    currentPreviewStream = streamName;

    // Refresh rows active status highlight
    document.querySelectorAll("#camera-tbody tr").forEach(row => {
        row.classList.remove("active-preview-row");
    });
    
    // Find row
    const rows = Array.from(document.querySelectorAll("#camera-tbody tr"));
    const activeRow = rows.find(r => r.cells[0]?.textContent === ip);
    if (activeRow) activeRow.classList.add("active-preview-row");

    const badgeIp = document.getElementById("previewing-camera-ip");
    badgeIp.textContent = ip;

    const iframe = document.getElementById("webrtc-iframe");
    const msg = document.getElementById("video-placeholder-msg");

    if (status === "Streaming" || status === "Grabbing") {
        const host = (globalConfig?.selectedInterfaceIp && globalConfig.selectedInterfaceIp !== "Auto")
            ? globalConfig.selectedInterfaceIp
            : (window.location.hostname || "localhost");
        // MediaMTX built-in WHEP/WebRTC player is exposed at http://<host>:8889/<stream_name>
        const webrtcUrl = `http://${host}:8889/${streamName}`;
        
        msg.style.display = "none";
        iframe.style.display = "block";
        if (iframe.src !== webrtcUrl) {
            iframe.src = webrtcUrl;
            appendLogLine(getCurrentTimeStr(), "INFO", `Loading WebRTC live preview for camera ${ip} (${streamName}).`);
        }
    } else {
        iframe.style.display = "none";
        iframe.src = "";
        msg.style.display = "flex";
        msg.querySelector("p").textContent = `Camera ${ip} is in '${status}' state (not streaming)`;
    }
}

// Service Commands AJAX
function triggerServiceControl(action) {
    appendLogLine(getCurrentTimeStr(), "INFO", `Triggering stream service ${action.toUpperCase()}...`);
    fetch(`/api/control/${action}`, { method: "POST" })
        .then(response => {
            if (!response.ok) throw new Error("HTTP error " + response.status);
            return response.json();
        })
        .then(data => {
            appendLogLine(getCurrentTimeStr(), "INFO", data.message || `Service ${action} triggered.`);
            fetchStatus();
        })
        .catch(err => {
            appendLogLine(getCurrentTimeStr(), "ERROR", `Failed to trigger service ${action}: ` + err.message);
        });
}

function restartCamera(event, ip) {
    event.stopPropagation(); // Avoid triggering row preview click
    appendLogLine(getCurrentTimeStr(), "INFO", `Triggering manual restart for camera ${ip}...`);
    fetch(`/api/control/restart-camera?ip=${ip}`, { method: "POST" })
        .then(response => {
            if (!response.ok) throw new Error("HTTP error " + response.status);
            return response.json();
        })
        .then(data => {
            appendLogLine(getCurrentTimeStr(), "INFO", data.message || `Camera ${ip} restart triggered.`);
        })
        .catch(err => {
            appendLogLine(getCurrentTimeStr(), "ERROR", `Failed to restart camera ${ip}: ` + err.message);
        });
}

// Settings Form Handling
function populateSettingsForm() {
    if (!globalConfig) return;

    document.getElementById("setting-ffmpeg").value = globalConfig.ffmpegPath;
    document.getElementById("setting-mediamtx").value = globalConfig.mediamtxPath;
    document.getElementById("setting-rtsp-port").value = globalConfig.mediamtxPort;
    document.getElementById("setting-web-port").value = globalConfig.webApiPort;

    const select = document.getElementById("setting-interface-ip");
    if (select) {
        select.value = globalConfig.selectedInterfaceIp || "Auto";
    }
}

function loadNetworkInterfaces() {
    return fetch("/api/interfaces")
        .then(response => {
            if (!response.ok) throw new Error("HTTP error " + response.status);
            return response.json();
        })
        .then(interfaces => {
            const select = document.getElementById("setting-interface-ip");
            if (!select) return;

            // Clear all except the first option (Auto)
            select.innerHTML = '<option value="Auto">Auto (Default Socket Routing)</option>';

            interfaces.forEach(ni => {
                const option = document.createElement("option");
                option.value = ni.ipAddress;
                option.textContent = `${ni.ipAddress} (${ni.description || ni.name})`;
                select.appendChild(option);
            });
        })
        .catch(err => {
            console.error("Failed to load network interfaces:", err);
        });
}

function renderSettingsCameraGrid() {
    const tbody = document.getElementById("settings-camera-tbody");
    if (!tbody) return;

    if (localConfigCameras.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-muted">No camera mappings configured. Click Add Camera.</td></tr>`;
        return;
    }

    let html = "";
    localConfigCameras.forEach((cam, index) => {
        html += `
            <tr>
                <td style="font-weight: 600;">${cam.ipAddress}</td>
                <td><code style="color: #3b82f6;">${cam.streamName}</code></td>
                <td>${cam.codec}</td>
                <td>${cam.encoderMode}</td>
                <td>${cam.targetFps}</td>
                <td>${cam.packetSize || 1500}</td>
                <td>${cam.interPacketDelay || 1000}</td>
                <td>
                    <div style="display: flex; gap: 6px;">
                        <button class="btn btn-secondary btn-xs" onclick="openCameraModal(${index})">Edit</button>
                        <button class="btn btn-danger btn-xs" onclick="deleteCamera(${index})">Delete</button>
                    </div>
                </td>
            </tr>
        `;
    });

    tbody.innerHTML = html;
}

// Add/Edit Camera Modal Dialog
function openCameraModal(index = -1) {
    const modal = document.getElementById("camera-modal");
    const title = document.getElementById("modal-title");
    const editIndex = document.getElementById("edit-index");

    editIndex.value = index;

    if (index === -1) {
        title.textContent = "Add New Camera Config";
        document.getElementById("input-ip").value = "";
        document.getElementById("input-stream").value = "";
        document.getElementById("input-codec").selectedIndex = 0; // H264
        document.getElementById("input-encoder").selectedIndex = 0; // CPU
        document.getElementById("input-fps").value = 30;
        document.getElementById("input-packet-size").value = 1500;
        document.getElementById("input-packet-delay").value = 1000;
    } else {
        const cam = localConfigCameras[index];
        title.textContent = "Edit Camera Config";
        document.getElementById("input-ip").value = cam.ipAddress;
        document.getElementById("input-stream").value = cam.streamName;
        document.getElementById("input-codec").value = cam.codec;
        document.getElementById("input-encoder").value = cam.encoderMode;
        document.getElementById("input-fps").value = cam.targetFps;
        document.getElementById("input-packet-size").value = cam.packetSize || 1500;
        document.getElementById("input-packet-delay").value = cam.interPacketDelay || 1000;
    }

    modal.classList.add("show");
}

function closeCameraModal() {
    const modal = document.getElementById("camera-modal");
    modal.classList.remove("show");
}

function submitCameraForm() {
    const index = parseInt(document.getElementById("edit-index").value);
    const ip = document.getElementById("input-ip").value.trim();
    const stream = document.getElementById("input-stream").value.trim();
    const codec = document.getElementById("input-codec").value;
    const encoder = document.getElementById("input-encoder").value;
    const fps = parseInt(document.getElementById("input-fps").value);
    const packetSize = parseInt(document.getElementById("input-packet-size").value) || 1500;
    const interPacketDelay = parseInt(document.getElementById("input-packet-delay").value) || 1000;

    // Simple IP Validation Regex
    const ipRegex = /^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$/;
    if (!ipRegex.test(ip)) {
        alert("Please enter a valid IPv4 address.");
        return;
    }

    if (!stream) {
        alert("RTSP path name cannot be empty.");
        return;
    }

    const camData = {
        ipAddress: ip,
        streamName: stream,
        codec: codec,
        encoderMode: encoder,
        targetFps: fps,
        packetSize: packetSize,
        interPacketDelay: interPacketDelay
    };

    if (index === -1) {
        // Add
        localConfigCameras.push(camData);
    } else {
        // Edit
        localConfigCameras[index] = camData;
    }

    renderSettingsCameraGrid();
    closeCameraModal();
}

function deleteCamera(index) {
    if (confirm(`Are you sure you want to delete camera mapping ${localConfigCameras[index].ipAddress}?`)) {
        localConfigCameras.splice(index, 1);
        renderSettingsCameraGrid();
    }
}

// Save Configurations
function saveConfiguration() {
    const ffmpeg = document.getElementById("setting-ffmpeg").value.trim();
    const mediamtx = document.getElementById("setting-mediamtx").value.trim();
    const rtspPort = parseInt(document.getElementById("setting-rtsp-port").value);
    const selectedIp = document.getElementById("setting-interface-ip").value;

    if (!ffmpeg || !mediamtx) {
        alert("FFmpeg and MediaMTX executable paths cannot be empty.");
        return;
    }

    const payload = {
        ffmpegPath: ffmpeg,
        mediamtxPath: mediamtx,
        mediamtxPort: rtspPort,
        webApiPort: 5000,
        selectedInterfaceIp: selectedIp,
        cameras: localConfigCameras
    };

    fetch("/api/config", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
    })
    .then(response => {
        if (!response.ok) throw new Error("HTTP error " + response.status);
        return response.json();
    })
    .then(data => {
        alert(data.message || "Configuration saved successfully.");
        // Fetch new status
        fetchStatus();
        switchTab("dashboard");
    })
    .catch(err => {
        alert("Failed to save configuration: " + err.message);
    });
}

// Utility Clipboard Copying
function copyToClipboard(event, text) {
    event.stopPropagation(); // Avoid selecting row preview click
    navigator.clipboard.writeText(text).then(() => {
        const btn = event.target;
        const originalText = btn.textContent;
        btn.textContent = "Copied!";
        btn.style.backgroundColor = "#10b981"; // change color to green
        setTimeout(() => {
            btn.textContent = originalText;
            btn.style.backgroundColor = ""; // reset color
        }, 1500);
    });
}

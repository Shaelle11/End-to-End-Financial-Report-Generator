document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("report_form");
  const reportTypeButtons = document.querySelectorAll(".segment_option");
  const reportYearSelect = document.getElementById("reportYear");
  const clientNameInput = document.getElementById("clientName");

  const loadingState = document.getElementById("loadingState");
  const successAlert = document.getElementById("successAlert");
  const errorAlert = document.getElementById("errorAlert");
  const successMessage = document.getElementById("successMessage");
  const errorMessage = document.getElementById("errorMessage");

  // Populate reporting years dynamically
  const currentYear = new Date().getFullYear();
  for (let y = currentYear; y >= currentYear - 5; y--) {
    const option = document.createElement("option");
    option.value = y;
    option.textContent = y;
    reportYearSelect.appendChild(option);
  }

  // Handle report type toggle
  reportTypeButtons.forEach((btn) => {
    btn.addEventListener("click", () => {
      reportTypeButtons.forEach((b) => b.classList.remove("active"));
      btn.classList.add("active");
    });
  });

  // Keyboard & accessibility support for report type buttons
  const options = document.querySelectorAll('.segment_option');
  options.forEach((btn, index) => {
    btn.addEventListener('click', () => {
      options.forEach(o => {
        o.classList.remove('active');
        o.setAttribute('aria-checked', 'false');
        o.setAttribute('tabindex', '-1');
      });
      btn.classList.add('active');
      btn.setAttribute('aria-checked', 'true');
      btn.setAttribute('tabindex', '0');
      btn.focus();
    });

    // Keyboard navigation
    btn.addEventListener('keydown', (e) => {
      let newIndex = index;
      if (e.key === 'ArrowRight' || e.key === 'ArrowDown') newIndex = (index + 1) % options.length;
      if (e.key === 'ArrowLeft' || e.key === 'ArrowUp') newIndex = (index - 1 + options.length) % options.length;
      if (newIndex !== index) {
        options[newIndex].click();
        e.preventDefault();
      }
    });
  });

  // Function to prepare and validate the request
  function prepareAgentRequest() {
    const activeBtn = document.querySelector(".segment_option.active");
    const reportType = activeBtn ? activeBtn.dataset.value : "";
    const reportYear = reportYearSelect.value.trim();
    const clientName = clientNameInput.value.trim();

    if (!reportType) {
      reportTypeButtons.forEach((b) => b.classList.add("shake"));
      setTimeout(() => reportTypeButtons.forEach((b) => b.classList.remove("shake")), 400);
      throw new Error("Please select a report type.");
    }
    if (!reportYear) throw new Error("Please select a reporting year.");
    if (!clientName) throw new Error("Please enter the client name or ID.");

    return { clientName, reportType, reportYear, timestamp: new Date().toISOString() };
  }

  // Handle form submission + backend request
  form.addEventListener("submit", async (e) => {
    e.preventDefault();
    successAlert.classList.remove("show");
    errorAlert.classList.remove("show");

    try {
      const payload = prepareAgentRequest();
      loadingState.classList.add("active");
      console.log("Sending payload:", payload);

      // Call your C# backend API here:
      const response = await fetch("http://localhost:5500/api/report/generate", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
      });

      if (!response.ok) throw new Error("Server returned an error. Please check backend logs.");
      const result = await response.json();

      loadingState.classList.remove("active");
      successMessage.textContent = result.message || "Report generated successfully!";
      successAlert.classList.add("show");
    } catch (err) {
      loadingState.classList.remove("active");
      errorMessage.textContent = err.message;
      errorAlert.classList.add("show");
    }
  });
});



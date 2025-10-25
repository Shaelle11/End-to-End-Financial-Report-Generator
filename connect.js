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

  // Populate years dynamically
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

  // ✅ Function to prepare and validate request
  function prepareAgentRequest() {
    const activeBtn = document.querySelector(".segment_option.active");
    const reportType = activeBtn ? activeBtn.dataset.value : "";
    const reportYear = reportYearSelect.value.trim();
    const clientName = clientNameInput.value.trim();

    if (!reportType) {
      reportTypeButtons.forEach((b) => b.classList.add("shake"));
      setTimeout(() => reportTypeButtons.forEach((b) => b.classList.remove("shake")), 400);
      throw new Error("Please select a report type before generating.");
    }

    if (!reportYear) throw new Error("Please select a reporting year.");
    if (!clientName) throw new Error("Please enter the client name or ID.");

    return {
      companyName: clientName,
      reportYear: parseInt(reportYear),
      totalRevenue: Math.floor(Math.random() * 1_000_000), // demo values
      totalExpense: Math.floor(Math.random() * 500_000),
      netProfit: Math.floor(Math.random() * 400_000),
    };
  }

  // 🧠 Submit and actually generate report
  form.addEventListener("submit", async (e) => {
    e.preventDefault();

    successAlert.classList.remove("show");
    errorAlert.classList.remove("show");

    try {
      const data = prepareAgentRequest();
      loadingState.classList.add("active");

      console.log("Sending payload:", data);

      const response = await fetch("http://localhost:5500/api/report/generate", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
      });

      if (!response.ok) throw new Error("Error generating report!");

      // Convert response to Blob and trigger download
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = "FinancialReport.docx";
      document.body.appendChild(a);
      a.click();
      a.remove();

      successMessage.textContent = `Report for ${data.companyName} (${data.reportYear}) successfully generated.`;
      successAlert.classList.add("show");
    } catch (err) {
      errorMessage.textContent = err.message;
      errorAlert.classList.add("show");
    } finally {
      loadingState.classList.remove("active");
    }
  });
});

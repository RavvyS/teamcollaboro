const state = {
  token: localStorage.getItem("token") || "",
  role: localStorage.getItem("role") || "",
  name: localStorage.getItem("name") || ""
};

const authInfo = document.getElementById("authInfo");
const output = document.getElementById("output");

setAuthInfo();

function setAuthInfo() {
  authInfo.textContent = state.token
    ? `Logged in as ${state.name} (${state.role})`
    : "Not logged in";
}

function headers(withJson = true) {
  const h = {};
  if (withJson) h["Content-Type"] = "application/json";
  if (state.token) h.Authorization = `Bearer ${state.token}`;
  return h;
}

async function api(path, options = {}) {
  const res = await fetch(path, options);
  const contentType = res.headers.get("content-type") || "";
  const body = contentType.includes("application/json") ? await res.json() : await res.text();
  if (!res.ok) throw new Error(typeof body === "string" ? body : JSON.stringify(body));
  return body;
}

function show(data) {
  output.textContent = typeof data === "string" ? data : JSON.stringify(data, null, 2);
}

document.getElementById("registerForm").addEventListener("submit", async (e) => {
  e.preventDefault();
  const payload = Object.fromEntries(new FormData(e.target).entries());
  try {
    show(await api("/api/auth/register", { method: "POST", headers: headers(), body: JSON.stringify(payload) }));
    e.target.reset();
  } catch (err) { show(err.message); }
});

document.getElementById("loginForm").addEventListener("submit", async (e) => {
  e.preventDefault();
  const payload = Object.fromEntries(new FormData(e.target).entries());
  try {
    const data = await api("/api/auth/login", { method: "POST", headers: headers(), body: JSON.stringify(payload) });
    state.token = data.token; state.role = data.role; state.name = data.name;
    localStorage.setItem("token", state.token);
    localStorage.setItem("role", state.role);
    localStorage.setItem("name", state.name);
    setAuthInfo();
    show(data);
  } catch (err) { show(err.message); }
});

document.getElementById("createBtn").addEventListener("click", async () => {
  const form = new FormData(document.getElementById("invoiceForm"));
  const payload = {
    invoiceNumber: form.get("invoiceNumber"),
    customerName: form.get("customerName"),
    amount: Number(form.get("amount")),
    status: form.get("status")
  };
  try {
    show(await api("/api/invoices", { method: "POST", headers: headers(), body: JSON.stringify(payload) }));
  } catch (err) { show(err.message); }
});

document.getElementById("updateBtn").addEventListener("click", async () => {
  const form = new FormData(document.getElementById("invoiceForm"));
  const invoiceId = form.get("invoiceId");
  if (!invoiceId) return show("Invoice ID is required for update");
  const payload = {
    invoiceNumber: form.get("invoiceNumber"),
    customerName: form.get("customerName"),
    amount: Number(form.get("amount")),
    status: form.get("status")
  };
  try {
    show(await api(`/api/invoices/${invoiceId}`, { method: "PUT", headers: headers(), body: JSON.stringify(payload) }));
  } catch (err) { show(err.message); }
});

document.getElementById("loadInvoicesBtn").addEventListener("click", async () => {
  try { show(await api("/api/invoices", { headers: headers(false) })); }
  catch (err) { show(err.message); }
});

document.getElementById("loadInvoiceBtn").addEventListener("click", async () => {
  const id = document.getElementById("invoiceByIdInput").value;
  if (!id) return show("Enter an invoice ID");
  try { show(await api(`/api/invoices/${id}`, { headers: headers(false) })); }
  catch (err) { show(err.message); }
});

document.getElementById("deleteInvoiceBtn").addEventListener("click", async () => {
  const id = document.getElementById("invoiceByIdInput").value;
  if (!id) return show("Enter an invoice ID");
  try {
    await api(`/api/invoices/${id}`, { method: "DELETE", headers: headers(false) });
    show(`Invoice ${id} deleted`);
  } catch (err) { show(err.message); }
});

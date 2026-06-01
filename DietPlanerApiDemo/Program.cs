using System.Net.Http.Json;
using System.Text.Json;

// ============================================================
//  DietPlaner REST API Demo
//  Uruchom aplikację DietPlaner przed odpaleniem tego programu
// ============================================================

const string BASE_URL  = "http://localhost:5056";
const string USERNAME  = "admin";
const string API_TOKEN = "a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4";

using var http = new HttpClient();
http.DefaultRequestHeaders.Add("X-Username", USERNAME);
http.DefaultRequestHeaders.Add("X-Api-Token", API_TOKEN);

Console.WriteLine("=== DietPlaner REST API Demo ===\n");

// ── 1. GET /api/diets ────────────────────────────────────────
Console.WriteLine("1. GET all diets:");
await PrintResponse(await http.GetAsync($"{BASE_URL}/api/diets"));

// ── 2. GET /api/wards ────────────────────────────────────────
Console.WriteLine("2. GET all wards:");
var wardsResponse = await http.GetAsync($"{BASE_URL}/api/wards");
var wardsJson = await wardsResponse.Content.ReadAsStringAsync();
PrintResponseWithBody(wardsResponse, wardsJson);

var wards = JsonSerializer.Deserialize<List<JsonElement>>(wardsJson);
int? wardId = wards?.Count > 0 ? wards[0].GetProperty("id").GetInt32() : null;

// ── 3. GET /api/patients ─────────────────────────────────────
Console.WriteLine("3. GET all patients (before add):");
await PrintResponse(await http.GetAsync($"{BASE_URL}/api/patients"));

// ── 4. POST /api/patients ────────────────────────────────────
Console.WriteLine("4. POST create new patient:");
var newPatient = new { name = "Jan", surname = "Testowy", pesel = "99010112345", dietId = (int?)1, wardId };
var postResponse = await http.PostAsJsonAsync($"{BASE_URL}/api/patients", newPatient);
var postJson = await postResponse.Content.ReadAsStringAsync();
PrintResponseWithBody(postResponse, postJson);

int createdId = JsonSerializer.Deserialize<JsonElement>(postJson).GetProperty("id").GetInt32();
Console.WriteLine($"   → Created patient with ID: {createdId}\n");

// ── 5. GET /api/patients/{id} ────────────────────────────────
Console.WriteLine($"5. GET patient/{createdId}:");
await PrintResponse(await http.GetAsync($"{BASE_URL}/api/patients/{createdId}"));

// ── 6. PUT /api/patients/{id} ────────────────────────────────
Console.WriteLine($"6. PUT update patient/{createdId} — change diet to ID 2:");
var updated = new { name = "Jan", surname = "Testowy", pesel = "99010112345", dietId = (int?)2, wardId };
await PrintResponse(await http.PutAsJsonAsync($"{BASE_URL}/api/patients/{createdId}", updated));

// ── 7. GET to confirm update ─────────────────────────────────
Console.WriteLine($"7. GET patient/{createdId} after update:");
await PrintResponse(await http.GetAsync($"{BASE_URL}/api/patients/{createdId}"));

// ── 8. DELETE /api/patients/{id} ────────────────────────────
Console.WriteLine($"8. DELETE patient/{createdId}:");
await PrintResponse(await http.DeleteAsync($"{BASE_URL}/api/patients/{createdId}"));

// ── 9. GET after delete — expect 404 ────────────────────────
Console.WriteLine($"9. GET patient/{createdId} after delete (expect 404):");
await PrintResponse(await http.GetAsync($"{BASE_URL}/api/patients/{createdId}"));

// ── 10. Unauthorized — wrong token ──────────────────────────
Console.WriteLine("10. Request with WRONG token (expect 401):");
using var badHttp = new HttpClient();
badHttp.DefaultRequestHeaders.Add("X-Username", "admin");
badHttp.DefaultRequestHeaders.Add("X-Api-Token", "wrong-token");
await PrintResponse(await badHttp.GetAsync($"{BASE_URL}/api/patients"));

Console.WriteLine("=== Demo complete ===");

// ─── Helpers ─────────────────────────────────────────────────
static async Task PrintResponse(HttpResponseMessage response)
{
    var body = await response.Content.ReadAsStringAsync();
    PrintResponseWithBody(response, body);
}

static void PrintResponseWithBody(HttpResponseMessage response, string body)
{
    var code = (int)response.StatusCode;
    Console.ForegroundColor = code < 300 ? ConsoleColor.Green
                            : code < 400 ? ConsoleColor.Yellow
                            : ConsoleColor.Red;
    Console.Write($"   [{code} {response.StatusCode}]");
    Console.ResetColor();

    if (!string.IsNullOrWhiteSpace(body) && body != "null")
    {
        try
        {
            var pretty = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<JsonElement>(body),
                new JsonSerializerOptions { WriteIndented = true });
            var preview = pretty.Length > 400 ? pretty[..400] + "..." : pretty;
            Console.WriteLine(" " + preview);
        }
        catch { Console.WriteLine(" " + body); }
    }
    else Console.WriteLine();
    Console.WriteLine();
}

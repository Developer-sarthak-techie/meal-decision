export async function POST(request: Request) {
  const body = (await request.json()) as { email?: string; password?: string; name?: string };
  if (!body.email || !body.password) {
    return Response.json({ error: "Email and password are required." }, { status: 400 });
  }

  return Response.json({
    user: {
      id: "u_1",
      name: body.name || "Home Chef",
      email: body.email,
    },
    token: "mock-token",
  });
}

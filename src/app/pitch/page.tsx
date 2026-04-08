import { readFileSync } from "fs";
import { join } from "path";

import Link from "next/link";

export default function PitchPage() {
  const markdown = readFileSync(join(process.cwd(), "pitch", "investor-pitch.md"), "utf8");

  return (
    <main className="min-h-screen px-4 py-10 sm:px-8">
      <div className="mx-auto max-w-3xl">
        <p className="mb-6 text-sm text-[var(--muted)]">
          <Link href="/" className="text-[var(--accent)] underline-offset-4 hover:underline">
            ← Back to app
          </Link>
        </p>
        <article className="glass rounded-3xl p-6 sm:p-8">
          <pre className="whitespace-pre-wrap font-sans text-sm leading-relaxed text-[var(--foreground)]">
            {markdown}
          </pre>
        </article>
      </div>
    </main>
  );
}

// ── Arquivo ───────────────────────────────────────────────────────────────

/** Corpo enviado ao registrar um arquivo vinculado a uma despesa. */
export interface ArquivoRequest {
  nome: string;
  extensao: string;
  /** Guid da despesa (template) que receberá o arquivo. */
  despesaID: string;
  /** Conteúdo do arquivo em base64 (sem prefixo data:). */
  arquivoByte: string;
  /** MIME type, ex.: application/pdf */
  content: string;
}

export interface ArquivoResponse {
  id: string;
  nome: string;
  dataUpload: string;
}

/** Lê um File e monta o payload esperado pela API. */
export async function buildArquivoRequest(
  file: File,
  despesaId: string,
): Promise<ArquivoRequest> {
  const arquivoByte = await fileToBase64(file);
  const extensao = extractExtensao(file.name);

  return {
    nome: file.name,
    extensao,
    despesaID: despesaId,
    arquivoByte,
    content: file.type || 'application/octet-stream',
  };
}

function extractExtensao(nome: string): string {
  const idx = nome.lastIndexOf('.');
  return idx >= 0 ? nome.slice(idx + 1) : '';
}

function fileToBase64(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result as string;
      const base64 = result.includes(',') ? result.split(',')[1] : result;
      resolve(base64 ?? '');
    };
    reader.onerror = () => reject(reader.error);
    reader.readAsDataURL(file);
  });
}

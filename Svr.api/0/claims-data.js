class SwapiService
{
  //_apiBase = 'https://localhost:44371/api/';
  _apiBase = 'https://localhost/api/';

  async  getClaims(url)
  {
    const res = await fetch(`${this._apiBase}${url}`);
    if (!res.ok)
    {
      throw new Error(`path: ${url}, status: ${res.status}`);
    }
    return await res.json();
  }

  async getAllClaims()
  {
    const res = await this.getClaims(`claims/`);
    return res.name;
  }

  getClaim(id)
  {
    return this.getClaims(`claims/${id}/`);
  }
}

export default function getClaims()
{
  const swapi = new SwapiService();
  swapi.getClaims().then((claims) =>
  {
    claims.forEach((c) =>
    {
      console.log(c.name);
    });
  });
  return;
}
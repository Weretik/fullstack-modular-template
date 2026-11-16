//@ts-check

/** @type {import('next').NextConfig} */
const nextConfig = {
  // The API client is a workspace library shipped as TypeScript source.
  transpilePackages: ['@template/api-client'],
  poweredByHeader: false,
};

module.exports = nextConfig;

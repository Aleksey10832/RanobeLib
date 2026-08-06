import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  allowedDevOrigins: ['192.168.1.186'],
  async rewrites() {
    return [
      {
        source: '/api/back/:path*',
        destination: 'http://localhost:9000/:path*', // Адрес вашего реального бэкенда
      },
    ];
  },
};

export default nextConfig;

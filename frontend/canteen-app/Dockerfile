# ---------- Build Stage ----------
FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install --legacy-peer-deps

COPY . .
RUN ls -la /app          # ← ADD THIS LINE (it will list all files)
# RUN npm run build --prod # depreciated
RUN npm run build -- --configuration production


# ---------- Runtime Stage ----------
FROM nginx:alpine
# COPY dist/canteen-app /usr/share/nginx/html # Not copying the build output correctly
COPY --from=build /app/dist/canteen-app /usr/share/nginx/html/
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

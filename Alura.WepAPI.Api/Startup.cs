using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Formatters;
using Alura.WepAPI.Api.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using Unchase.Swashbuckle.AspNetCore.Extensions.Filters;

namespace Alura.WepAPI.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LeituraContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ListaLeitura"));
            });

            services.AddTransient<IRepository<Livro>, RepositorioBaseEF<Livro>>();

            services.AddMvc(opt =>
            {
                opt.OutputFormatters.Add(new LivroCsvFormatter());
                opt.Filters.Add(typeof(ErrorResponseExceptionFilter));
            }).AddXmlSerializerFormatters();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = "JwtBearer";
                opt.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(@"$_Qr@2uGWjjU=2^qy5DpaYYd2WPL%UG$pK*-Eb%^gdh%wtZR3F6+ydNVRVE+hsZShe-YaJA68YRLjMTx==Utry-fXUEV$tfvbEY4SRNw%xqm7VVF%h#cc!ysv5^7yLESqF*hsv=sy^%4C3x@a7Kc_BHr5?5FyvcWTwTYnzg^rd?^u!XUu+%N7v=akMZLMaX769U6bHZQQu^2D_N!nHM-G82DFB&zpqKeFpq9EHTngPwpkJzJj5-$rM=p&4#+rATx")),
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidIssuer = "Alura.WebApp",
                    ValidAudience = "Postman",
                };
            });

            //Microsoft.AspNetCore.Mvc.Versioning
            //Adiciona versionamento de API (Rota, Query String, Header, etc)
            /*
             * Por rota não é compatível com outro
             * Query String e Header podem ser usados juntos
             * Por padrão (sem rota), já aceita por query string
             */


            //services.AddApiVersioning();

            //services.AddApiVersioning(opt => 
            //opt.ApiVersionReader = new HeaderApiVersionReader("api-version"));

            //services.AddApiVersioning(opt => 
            //    opt.ApiVersionReader = ApiVersionReader.Combine(  
            //        new HeaderApiVersionReader("api-version"),
            //        new QueryStringApiVersionReader("api-version")
            //        )
            //    );

            services.AddApiVersioning();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Documentação API Livros V1.0", Description = "Documentação da API", Version = "1.0" });
                c.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Documentação API Livros V2.0", Description = "Documentação da API", Version = "1.0" });
                c.EnableAnnotations();

                var openApiSecurityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Description = "Autenticação Bearer via JWT"

                };

                c.AddSecurityDefinition("Bearer", openApiSecurityScheme);

                var openApiSecurityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement();
                openApiSecurityRequirement.Add(openApiSecurityScheme, new List<string>());

                c.AddSecurityRequirement(openApiSecurityRequirement);

                c.AddEnumsWithValuesFixFilters(o =>
                {
                    o.ApplySchemaFilter = true;
                });
                //c.SchemaFilter<AutoRestSchemaFilter>(); /* Não funcionou */

                c.OperationFilter<AuthResponsesOperationFilter>();

                c.DocumentFilter<TagDescriptionsDocumentFilter>();

                c.DescribeAllParametersInCamelCase();

                /* c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); */
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                s.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
            });
            
        }
    }
}
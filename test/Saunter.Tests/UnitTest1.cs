using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using Saunter.AsyncApiSchema.v2_0_0;
using Saunter.Microsoft.Extensions.DependencyInjection;

namespace Saunter.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var services = new ServiceCollection() as IServiceCollection;
            services.AddSaunter(
                options =>
                {
                    options.AsyncApiSchema = new AsyncApiSchema.v2_0_0.AsyncApiSchema
                    {
                        Id = new Identifier("urn:com:example:example-events"),
                        Info = new Info("Example API", "An example API with events")
                        {
                            Version = "2019.01.12345",
                            Contact = new Contact
                            {
                                Email = "michael@mwild.me",
                                Name = "Michael Wildman",
                                Url = "https://mwild.me/",
                            },
                            License = new License("MIT"),
                            TermsOfService = "https://mwild.me/tos",
                        },
                        Tags = new HashSet<Tag>{ new Tag("example"), new Tag("event") },
                        Servers = new Servers
                        {
                            { 
                                new ServersFieldName("development"), 
                                new Server("rabbitmq.dev.mwild.me", "amqp")
                                {
                                    Security = new List<SecurityRequirement> { new SecurityRequirement { { "user-password", new List<string>() } }}
                                }
                            }
                        },
                        Components = new Components
                        {
                            SecuritySchemes = new Dictionary<ComponentFieldName, SecurityScheme>
                            {
                                { new ComponentFieldName("user-password"), new SecurityScheme(SecuritySchemeType.Http) }
                            }
                        }
                    };
                });

            var sp = services.BuildServiceProvider();

            var provider = sp.GetRequiredService<IAsyncApiSchemaProvider>();

            var schema = provider.GetSchema();
            var json = JsonConvert.SerializeObject(schema, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            
            Assert.That(schema, Is.Not.Null);

        }
    }

    public class ExampleMessage
    {
        public string SomeProperty { get; set; }

        public int Abc { get; set; }
    }
    
    
}